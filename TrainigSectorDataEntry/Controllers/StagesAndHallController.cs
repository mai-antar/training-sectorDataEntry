
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
        private readonly IGenericService<StagesAndHallsImage> _StagesAndHallsImageService;
        private readonly IGenericService<TrainingSector> _TrainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;

        public StagesAndHallController(IGenericService<StagesAndHall> StagesAndHallService,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService, IGenericService<StagesAndHallsImage> stagesAndHallsImageService)
        {
            _StagesAndHallService = StagesAndHallService;
            _TrainingSectorService = TrainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _StagesAndHallsImageService = stagesAndHallsImageService;
        }
        public async Task<IActionResult> Index()
        {
            var StagesAndHallList = await _StagesAndHallService.GetAllAsync();
            var StagesAndHallImageList = await _StagesAndHallsImageService.GetAllAsync();

            var sectors = await _TrainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");

            foreach (var item in StagesAndHallList)
            {
                if (StagesAndHallImageList.Where(a => a.StagesAndHallsId == item.Id).ToList().Count > 0)
                {

                    item.StagesAndHallsImages = StagesAndHallImageList.Where(a => a.StagesAndHallsId == item.Id).ToList();
                }
            }


            var viewModelList = _mapper.Map<List<StagesAndHallVM>>(StagesAndHallList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            var StagesAndHallImageList = await _StagesAndHallsImageService.GetAllAsync();
            var existingStagesAndHall = await _StagesAndHallService.GetAllAsync();
            var existingStagesAndHallVM = _mapper.Map<List<StagesAndHallVM>>(existingStagesAndHall);

            foreach (var item in existingStagesAndHall)
            {
                if (StagesAndHallImageList.Where(a => a.StagesAndHallsId == item.Id).ToList().Count > 0)
                {

                    item.StagesAndHallsImages = StagesAndHallImageList.Where(a => a.StagesAndHallsId == item.Id).ToList();
                }
            }

            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            ViewBag.ExistingStagesAndHall = existingStagesAndHallVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StagesAndHallVM model)
        {
            // Validate that an image is uploaded
            //if (model.UploadedImages == null || model.UploadedImages.Any())
            //{
            //    ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            //}

            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

                var existingStagesAndHall = await _StagesAndHallService.GetAllAsync();
                var existingStagesAndHallVM = _mapper.Map<List<StagesAndHallVM>>(existingStagesAndHall);

                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                ViewBag.ExistingStagesAndHall = existingStagesAndHallVM;

                return View(model);
            }

            var entity = _mapper.Map<StagesAndHall>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

            await _StagesAndHallService.AddAsync(entity);
            // Save the image

            if (model.UploadedImages != null)
            {
                foreach (var file in model.UploadedImages)
                { 

                    var relativePath = await _fileStorageService.UploadImageAsync(file, "StagesAndHallImage");

                if (relativePath != null)
                {
                        await _StagesAndHallsImageService.AddAsync(new StagesAndHallsImage
                        {
                            StagesAndHallsId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });

                    }
            }
            }

            TempData["Success"] = "تمت الاضافة بنجاح";

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
     
            var StagesAndHall = await _StagesAndHallService.GetByIdAsync(id, n => ((StagesAndHall)n).StagesAndHallsImages);
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

            //// If no new image uploaded AND no existing image, throw validation error
            //if (model.UploadedImages == null && string.IsNullOrEmpty(entity.ImagePath))
            //{
            //    ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            //    return View(model);
            //}


            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.IsActive = model.IsActive;
            entity.ISStage = model.ISStage;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

        
            await _StagesAndHallService.UpdateAsync(entity);

            if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
            {
                foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
                {
                    var image = await _StagesAndHallsImageService.GetByIdAsync(id);
                    if (image != null)
                    {
                        await _fileStorageService.DeleteFileAsync(image.ImagePath);
                        await _StagesAndHallsImageService.DeleteAsync(id);
                   
                    }
                }
            }

            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
                foreach (var image in model.UploadedImages)
                {

                    var relativePath = await _fileStorageService.UploadImageAsync(image, "StagesAndHallImage");

                    if (relativePath != null)
                    {
                        await _StagesAndHallsImageService.AddAsync(new StagesAndHallsImage
                        {
                            StagesAndHallsId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });

                    }
                }
            }
            

            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> AddImages(int id)
        {
            var StagesAndHallsImage = await _StagesAndHallService.GetByIdAsync(id);
            if (StagesAndHallsImage == null)
                return NotFound();

            var vm = new StagesAndHallsImageVM
            {
                StagesAndHallsId = id,
                Name = StagesAndHallsImage.TitleAr
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(StagesAndHallsImageVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var news = await _StagesAndHallService.GetByIdAsync(model.StagesAndHallsId);
            if (news == null)
                return NotFound();


            foreach (var image in model.UploadedImages)
            {
                var relativePath = await _fileStorageService.UploadImageAsync(image, "StagesAndHallsImages");

                if (relativePath != null)
                {
                    await _StagesAndHallsImageService.AddAsync(new StagesAndHallsImage
                    {
                        StagesAndHallsId = model.Id,
                        ImagePath = relativePath,
                        IsActive = true,
                        IsDeleted = false,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                    });
                }

            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _StagesAndHallsImageService.GetByIdAsync(id);
            if (image == null) return NotFound();

            await _fileStorageService.DeleteFileAsync(image.ImagePath);
            await _StagesAndHallsImageService.DeleteAsync(id);
            return RedirectToAction("Edit", new { id = image.StagesAndHallsId });
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
