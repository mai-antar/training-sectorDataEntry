
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class StagesAndHallController : Controller
    {
        private readonly IGenericService<StagesAndHall> _StagesAndHallService;
        private readonly IGenericService<TrainingSector> _TrainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public StagesAndHallController(IGenericService<StagesAndHall> StagesAndHallService,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger)
        {
            _StagesAndHallService = StagesAndHallService;
            _TrainingSectorService = TrainingSectorService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var StagesAndHallList = await _StagesAndHallService.GetAllAsync();
     

            var sectors = await _TrainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
         
            var viewModelList = _mapper.Map<List<StagesAndHallVM>>(StagesAndHallList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            var existingStagesAndHall = await _StagesAndHallService.GetAllAsync();
            var existingStagesAndHallVM = _mapper.Map<List<StagesAndHallVM>>(existingStagesAndHall);

  
       
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            ViewBag.ExistingStagesAndHall = existingStagesAndHallVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StagesAndHallVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

                var existingStagesAndHall = await _StagesAndHallService.GetAllAsync();
                var existingStagesAndHallVM = _mapper.Map<List<StagesAndHallVM>>(existingStagesAndHall);

                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                ViewBag.ExistingStagesAndHall = existingStagesAndHallVM;

                return View(model);
            }
            // Save the image
            string? fileName = null;
            if (model.UploadedImage != null)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/StagesAndHallImage");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                if (model.UploadedImage.Length > 0 && model.UploadedImage.ContentType.StartsWith("image/"))
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UploadedImage.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedImage.CopyToAsync(stream);
                    }
                }
            }

            var entity = _mapper.Map<StagesAndHall>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
            entity.ImagePath = "/uploads/StagesAndHallImage/" + fileName;
            await _StagesAndHallService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var StagesAndHall = await _StagesAndHallService.GetByIdAsync(id);
            if (StagesAndHall == null) return NotFound();

            var model = _mapper.Map<StagesAndHallVM>(StagesAndHall);
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StagesAndHallVM model)
        {
            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                return View(model);
            }

            var entity = await _StagesAndHallService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            // If no new image uploaded AND no existing image, throw validation error
            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }



            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.IsActive = model.IsActive;
            entity.ISStage = model.ISStage;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);


            if (model.UploadedImage != null && model.UploadedImage.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entity.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/StagesAndHallImage");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UploadedImage.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadedImage.CopyToAsync(stream);
                }

                // Update entity path
                entity.ImagePath = "/uploads/StagesAndHallImage/" + fileName;
            }

            // Ensure image path is still set
            if (string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                return View(model);
            }

            await _StagesAndHallService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var StagesAndHall = await _StagesAndHallService.GetByIdAsync(id);
            if (StagesAndHall == null) return NotFound();

            await _StagesAndHallService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
