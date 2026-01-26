
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.Services;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class StagesAndHallController : Controller
    {
        private readonly IGenericService<StagesAndHall> _StagesAndHallService;
        private readonly IGenericService<TrainingSector> _TrainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;

        public StagesAndHallController(IGenericService<StagesAndHall> StagesAndHallService,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _StagesAndHallService = StagesAndHallService;
            _TrainingSectorService = TrainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
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

            if (model.UploadedImage != null)
            {


                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "StagesAndHallImage");

                if (relativePath != null)
                {
                    await _StagesAndHallService.AddAsync(new StagesAndHall
                    {
                        TrainigSectorId = model.TrainigSectorId,
                        DescriptionAr = model.DescriptionAr,
                        DescriptionEn = model.DescriptionEn,
                        IsDeleted = false,
                        IsActive = true,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today),
                        ImagePath = relativePath
                    });

                }

            }

            TempData["Success"] = "تمت الاضافة بنجاح";

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

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(entity.ImagePath);
                }

                var relativePath = await _fileStorageService
                    .UploadImageAsync(model.UploadedImage, "StagesAndHallImage");

                entity.ImagePath = relativePath;

                if (string.IsNullOrEmpty(relativePath))
                {
                    ModelState.AddModelError("UploadedImage", "حدث خطأ أثناء رفع الصورة.");
                    var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

                    ViewBag.TrainingSectorList =
                        new SelectList(TrainingSector, "Id", "NameAr");
                    return View(model);
                }

                entity.ImagePath = relativePath;
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

            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var StagesAndHall = await _StagesAndHallService.GetByIdAsync(id);
            if (StagesAndHall == null) return NotFound();

            await _StagesAndHallService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetStagesAndHallByTrainingSectorId(int trainingSectorId)
        {

            var sectors = await _TrainingSectorService.GetDropdownListAsync();
            var StagesAndHall = await _StagesAndHallService.GetAllAsync();
            StagesAndHall = StagesAndHall.Where(a => a.TrainigSectorId == trainingSectorId).ToList();

            var vmList = _mapper.Map<List<StagesAndHallVM>>(StagesAndHall);


            return PartialView("_StagesAndHallPartial", vmList);
        }
    }
}
