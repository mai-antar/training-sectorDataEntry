
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.DataContext;
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
        private readonly IEntityImageService _entityImageService;
        private readonly TrainingSectorDbContext _context;

        public StagesAndHallController(IGenericService<StagesAndHall> StagesAndHallService,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService, 
            IGenericService<StagesAndHallsImage> stagesAndHallsImageService, IEntityImageService entityImageService, TrainingSectorDbContext context)
        {
            _StagesAndHallService = StagesAndHallService;
            _TrainingSectorService = TrainingSectorService;
            _entityImageService = entityImageService;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _fileStorageService = fileStorageService;
            _StagesAndHallsImageService = stagesAndHallsImageService;
        }
        public async Task<IActionResult> Index()
        {
            var StagesAndHallList = await _StagesAndHallService.GetAllAsync();

            var StagesAndHallImageList = await _entityImageService.FindAsync(
           x => x.EntityImagesTableTypeId == 3 && x.IsDeleted != true);

            var sectors = await _TrainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");

       

            var viewModelList = _mapper.Map<List<StagesAndHallVM>>(StagesAndHallList);
            foreach (var item in viewModelList)
            {
                if (StagesAndHallImageList.Where(a => a.EntityId == item.Id).ToList().Count > 0)
                {

                    item.StagesAndHallsImages = StagesAndHallImageList.Where(a => a.EntityId == item.Id).ToList();
                }
            }
            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");

            var StagesAndHallImageList = await _entityImageService.FindAsync(
                 x => x.EntityImagesTableTypeId == 3 && x.IsDeleted == false
             );

            var existingStagesAndHall = await _StagesAndHallService.GetAllAsync();
            var existingStagesAndHallVM = _mapper.Map<List<StagesAndHallVM>>(existingStagesAndHall);


            //  Attach images to each project
            foreach (var StagesAndHall in existingStagesAndHallVM)
            {
                StagesAndHall.StagesAndHallsImages = StagesAndHallImageList
                    .Where(x => x.EntityId == StagesAndHall.Id)
                    .ToList();
            }

            // Preserve selected facility
            if (TempData["StagesAndHall_TrainingSectorId"] != null)
            {
                ViewBag.TrainingSectorList = new SelectList(
                    TrainingSector,
                    "Id",
                    "NameAr",
                    TempData["StagesAndHall_TrainingSectorId"]);
            }


            ViewBag.ExistingStagesAndHall = existingStagesAndHallVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StagesAndHallVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = _mapper.Map<StagesAndHall>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

                await _StagesAndHallService.AddAsync(entity);

                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        3,
                        entity.Id,
                        model.UploadedImages);
                }


                await transaction.CommitAsync();

                TempData["Success"] = "تمت الإضافة بنجاح";
                TempData["StagesAndHall_TrainingSectorId"] = model.TrainigSectorId;
                return RedirectToAction(nameof(Create));

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(StagesAndHallController), nameof(Create));
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ، تم إلغاء العملية.");

                return View(model);
            }

           
        }


        public async Task<IActionResult> Edit(int id)
        {
     
           
            var StagesAndHall = await _StagesAndHallService.GetByIdAsync(id);
            if (StagesAndHall == null) return NotFound();

            var model = _mapper.Map<StagesAndHallVM>(StagesAndHall);

            var StagesAndHallImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 3 && x.EntityId == id && x.IsDeleted == false);

            model.StagesAndHallsImages = StagesAndHallImages;

            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StagesAndHallVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _StagesAndHallService.GetByIdAsync(model.Id);
                if (entity == null) return NotFound();

                entity.TitleAr = model.TitleAr;
                entity.TitleEn = model.TitleEn;
                entity.DescriptionAr = model.DescriptionAr;
                entity.DescriptionEn = model.DescriptionEn;
                entity.TrainigSectorId = model.TrainigSectorId;
                entity.IsActive = model.IsActive;
                entity.ISStage = model.ISStage;
                entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);


                await _StagesAndHallService.UpdateAsync(entity);

                //  حذف صور
                if (model.DeletedImageIds != null)
                {
                    foreach (var imageId in model.DeletedImageIds
                        .Where(x => x.HasValue)
                        .Select(x => x.Value))
                    {
                        await _entityImageService.DeleteImageAsync(imageId);
                    }
                }

                //  إضافة صور
                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        1,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();


                TempData["Success"] = "تم التعديل بنجاح";

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(StagesAndHallController), nameof(Edit));
                ModelState.AddModelError("", "حدث خطأ أثناء التعديل، تم إلغاء العملية.");

                return View(model);
            }
           
        }
 
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {

            var image = await _entityImageService.GetByIdAsync(id);
            if (image == null) return NotFound();
            await _entityImageService.DeleteImageAsync(id);

            return RedirectToAction("Edit", new { id = image.EntityId });
        }
        public async Task<IActionResult> Delete(int id)
        {
            var StagesAndHall = await _StagesAndHallService.GetByIdAsync(id);
            if (StagesAndHall == null) return NotFound();

            // Delete associated images from file system
            var StagesAndHallImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 3 && x.EntityId == id && x.IsDeleted == false);
            if (StagesAndHallImages != null && StagesAndHallImages.Any())
            {
                foreach (var img in StagesAndHallImages)
                    await _fileStorageService.DeleteFileAsync(img.ImagePath);
            }


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
