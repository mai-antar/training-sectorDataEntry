using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public StudentActiviteController(IGenericService<StudentActivite> StudentActiviteService,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _StudentActiviteService = StudentActiviteService;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var StudentActiviteList = await _StudentActiviteService.GetAllAsync();
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<StudentActiviteVM>>(StudentActiviteList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingStudentActivite = await _StudentActiviteService.GetAllAsync();
            var existingStudentActiviteVM = _mapper.Map<List<StudentActiviteVM>>(existingStudentActivite);



            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.existingStudentActivite = existingStudentActiviteVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentActiviteVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingStudentActivite = await _StudentActiviteService.GetAllAsync();
                var existingStudentActiviteVM = _mapper.Map<List<StudentActiviteVM>>(existingStudentActivite);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingStudentActivite = existingStudentActiviteVM;

                return View(model);
            }

            // Save the image
            if (model.UploadedImage != null)
            {


                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "StudentActiviteImage");

                if (relativePath != null)
                {
                    await _StudentActiviteService.AddAsync(new StudentActivite
                    {
                        EducationalFacilitiesId = model.EducationalFacilitiesId,
                        DescriptionAr = model.DescriptionAr,
                        DescriptionEn = model.DescriptionEn,
                        IsDeleted = false,
                        IsActive = true,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today),
                        ImagePath = relativePath
                    });

                }

            }

            
    

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var StudentActivite = await _StudentActiviteService.GetByIdAsync(id);
            if (StudentActivite == null) return NotFound();

            var model = _mapper.Map<StudentActiviteVM>(StudentActivite);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentActiviteVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            var entity = await _StudentActiviteService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            // If no new image uploaded AND no existing image, throw validation error
            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }



            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

            if (model.UploadedImage != null && model.UploadedImage.Length > 0)
            {

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(entity.ImagePath);
                }

                var relativePath = await _fileStorageService
                    .UploadImageAsync(model.UploadedImage, "StudentActiviteImage");

                entity.ImagePath = relativePath;

                if (string.IsNullOrEmpty(relativePath))
                {
                    ModelState.AddModelError("UploadedImage", "حدث خطأ أثناء رفع الصورة.");
                    var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                    ViewBag.educationalFacilityList =
                        new SelectList(educationalFacility, "Id", "NameAr");
                    return View(model);
                }

                entity.ImagePath = relativePath;
            }


            // Ensure image path is still set
            if (string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            await _StudentActiviteService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var StudentActivite = await _StudentActiviteService.GetByIdAsync(id);
            if (StudentActivite == null) return NotFound();

            await _StudentActiviteService.DeleteAsync(id);
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
