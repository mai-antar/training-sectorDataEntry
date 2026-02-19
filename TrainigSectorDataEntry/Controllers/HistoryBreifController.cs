using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;
using TrainigSectorDataEntry.DataContext;
namespace TrainigSectorDataEntry.Controllers
{
    public class HistoryBreifController : Controller
    {
        private readonly IGenericService<HistoryBreif> _historyBreifService;
        private readonly IGenericService<HistoryBerifImage> _historyBerifImageService;
        private readonly IEntityImageService _entityImageService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly TrainingSectorDbContext _context;
        public HistoryBreifController(IGenericService<HistoryBreif> historyBreifService, IGenericService<HistoryBerifImage> historyBerifImageService, 
            IGenericService<EducationalFacility> educationalFacilityService, 
            IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService,
            IEntityImageService entityImageService, TrainingSectorDbContext context)
        {
            _historyBreifService = historyBreifService;
            _historyBerifImageService = historyBerifImageService;
            _educationalFacilityService = educationalFacilityService;
            _entityImageService = entityImageService;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var historyBreifList = await _historyBreifService.GetAllAsync();

            var historyBerifImageList = await _entityImageService.FindAsync(
            x => x.EntityImagesTableTypeId == 4 && x.IsDeleted != true);

        

            var sectors = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<HistoryBreifVM>>(historyBreifList);

            foreach (var item in viewModelList)
            {
                if (historyBerifImageList.Where(a => a.EntityId == item.Id).ToList().Count > 0)
                {

                    item.HistoryBerifImages = historyBerifImageList.Where(a => a.EntityId == item.Id).ToList();
                }
            }

            

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var existingHistoryBreif = await _historyBreifService.GetAllAsync();
            var existingHistoryBreifVM = _mapper.Map<List<HistoryBreifVM>>(existingHistoryBreif);

            var HistoryBreifImages = await _entityImageService.FindAsync(
                  x => x.EntityImagesTableTypeId == 4 && x.IsDeleted == false
              );

            //  Attach images to each project
            foreach (var HistoryBreif in existingHistoryBreifVM)
            {
                HistoryBreif.HistoryBerifImages = HistoryBreifImages
                    .Where(x => x.EntityId == HistoryBreif.Id)
                    .ToList();
            }

            // Preserve selected facility
            if (TempData["HistoryBreif_EducationalFacilitiesId"] != null)
            {
                ViewBag.educationalFacilityList = new SelectList(
                    educationalFacility,
                    "Id",
                    "NameAr",
                    TempData["HistoryBreif_EducationalFacilitiesId"]);
            }

          
            ViewBag.ExistingHistoryBreif = existingHistoryBreifVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HistoryBreifVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = _mapper.Map<HistoryBreif>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

                await _historyBreifService.AddAsync(entity);

                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        4,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();

                TempData["Success"] = "تمت الإضافة بنجاح";
                TempData["HistoryBreif_EducationalFacilitiesId"] = model.EducationalFacilitiesId;
                return RedirectToAction(nameof(Create));

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(HistoryBreifController), nameof(Create));
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ، تم إلغاء العملية.");

                return View(model);
            }


          
        }

        public async Task<IActionResult> Edit(int id)
        {
            var HistoryBreif = await _historyBreifService.GetByIdAsync(id);
           
            if (HistoryBreif == null) return NotFound();

            var model = _mapper.Map<HistoryBreifVM>(HistoryBreif);

            var HistoryBreifImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 4 && x.EntityId == id && x.IsDeleted == false);

            model.HistoryBerifImages = HistoryBreifImages;

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HistoryBreifVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _historyBreifService.GetByIdAsync(model.Id);
                if (entity == null) return NotFound();

                entity.TitleAr = model.TitleAr;
                entity.TitleEn = model.TitleEn;
                entity.NameAr = model.NameAr;
                entity.NameEn = model.NameEn;
                entity.DescriptionAr = model.DescriptionAr;
                entity.DescriptionEn = model.DescriptionEn;
                entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
                entity.IsActive = model.IsActive;
                entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



                await _historyBreifService.UpdateAsync(entity);


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

                _logger.LogError(ex, nameof(ProjectsController), nameof(Edit));
                ModelState.AddModelError("", "حدث خطأ أثناء التعديل، تم إلغاء العملية.");

                return View(model);
            }


           
        }
   
   
        public async Task<IActionResult> Delete(int id)
        {
            var historyBreif = await _historyBreifService.GetByIdAsync(id);
            if (historyBreif == null) return NotFound();

        
            // Delete associated images from file system
            var historyBreifImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 4 && x.EntityId == id && x.IsDeleted == false);
            if (historyBreifImages != null && historyBreifImages.Any())
            {
                foreach (var img in historyBreifImages)
                    await _fileStorageService.DeleteFileAsync(img.ImagePath);
            }
            await _historyBreifService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetHistoryBreifByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var historyBreif = await _historyBreifService.GetAllAsync();

            historyBreif = historyBreif.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<HistoryBreifVM>>(historyBreif);
            var historyBreifImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId ==4 && x.IsDeleted == false);

            foreach (var vm in vmList)
            {
                vm.HistoryBerifImages = historyBreifImages.Where(x => x.EntityId == vm.Id).ToList();
            }

            return PartialView("_HistoryBreifPartial", vmList);
        }

    }
}
