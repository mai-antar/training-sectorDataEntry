using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.DataContext;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class StudentActiviteController : Controller
    {

        private readonly IGenericService<StudentActivite> _StudentActiviteService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEntityImageService _entityImageService;
        private readonly TrainingSectorDbContext _context;

        public StudentActiviteController(IGenericService<StudentActivite> StudentActiviteService,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService
            , IEntityImageService entityImageService, TrainingSectorDbContext context)
        {
            _StudentActiviteService = StudentActiviteService;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _entityImageService = entityImageService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var StudentActiviteList = await _StudentActiviteService.GetAllAsync();

            var StudentActiviteImagesList = await _entityImageService.FindAsync(
               x => x.EntityImagesTableTypeId == 6 && x.IsDeleted != true);

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<StudentActiviteVM>>(StudentActiviteList);

            foreach (var item in viewModelList)
            {
                if (StudentActiviteImagesList.Where(a => a.EntityId == item.Id).ToList().Count > 0)
                {

                    item.StudentActiviteImages = StudentActiviteImagesList.Where(a => a.EntityId == item.Id).ToList();
                }
            }

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var existingStudentActivite = await _StudentActiviteService.GetAllAsync();
            var existingStudentActiviteVM = _mapper.Map<List<StudentActiviteVM>>(existingStudentActivite);


            var StudentActiviteImages = await _entityImageService.FindAsync(
           x => x.EntityImagesTableTypeId == 6 && x.IsDeleted == false
       );

          
            foreach (var project in existingStudentActiviteVM)
            {
                project.StudentActiviteImages = StudentActiviteImages
                    .Where(x => x.EntityId == project.Id)
                    .ToList();
            }

            if (TempData["StudentActivite_EducationalFacilitiesId"] != null)
            {
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr", TempData["StudentActivite_EducationalFacilitiesId"]);
            }

            ViewBag.existingStudentActivite = existingStudentActiviteVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentActiviteVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                // Validate that an image is uploaded
                if (model.UploadedImage == null || model.UploadedImage.Length == 0)
                {
                    ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                }

                var entity = _mapper.Map<StudentActivite>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

                await _StudentActiviteService.AddAsync(entity);

                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        1,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();

                TempData["Success"] = "تمت الاضافة بنجاح";
                TempData["StudentActivite_EducationalFacilitiesId"] = model.EducationalFacilitiesId;
                return RedirectToAction(nameof(Create));

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(StudentActiviteController), nameof(Create));
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ، تم إلغاء العملية.");

                return View(model);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var StudentActivite = await _StudentActiviteService.GetByIdAsync(id);
            if (StudentActivite == null) return NotFound();

            var model = _mapper.Map<StudentActiviteVM>(StudentActivite);

            var StudentActiviteImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 6 && x.EntityId == id && x.IsDeleted == false);

            model.StudentActiviteImages = StudentActiviteImages;

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentActiviteVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _StudentActiviteService.GetByIdAsync(model.Id);
                if (entity == null) return NotFound();

             
                entity.DescriptionAr = model.DescriptionAr;
                entity.DescriptionEn = model.DescriptionEn;
                entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
                entity.IsActive = model.IsActive;
                entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

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
                        6,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();



                await _StudentActiviteService.UpdateAsync(entity);

                TempData["Success"] = "تم التعديل بنجاح";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(StudentActiviteController), nameof(Edit));
                ModelState.AddModelError("", "حدث خطأ أثناء التعديل، تم إلغاء العملية.");

                return View(model);
            }
          
        }

        public async Task<IActionResult> Delete(int id)
        {
            var StudentActivite = await _StudentActiviteService.GetByIdAsync(id);
            if (StudentActivite == null) return NotFound();

            // Delete associated images from file system
            var StudentActiviteImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 6 && x.EntityId == id && x.IsDeleted == false);

            if (StudentActiviteImages != null && StudentActiviteImages.Any())
            {
                foreach (var img in StudentActiviteImages)
                    await _fileStorageService.DeleteFileAsync(img.ImagePath);
            }

            await _StudentActiviteService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentActiviteByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var alerts = await _StudentActiviteService.GetAllAsync();
            alerts = alerts.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<StudentActiviteVM>>(alerts);


            return PartialView("_StudentActivitePartial", vmList);
        }
    }
}
