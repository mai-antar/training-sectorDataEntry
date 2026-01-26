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
    public class QualityCertificateController : Controller
    {
        private readonly IGenericService<QualityCertificate> _QualityCertificateService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public QualityCertificateController(IGenericService<QualityCertificate> QualityCertificateService,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _QualityCertificateService = QualityCertificateService;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var QualityCertificateList = await _QualityCertificateService.GetAllAsync();
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
  
            var viewModelList = _mapper.Map<List<QualityCertificateVM>>(QualityCertificateList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingQualityCertificate = await _QualityCertificateService.GetAllAsync();
            var existingQualityCertificateVM = _mapper.Map<List<QualityCertificateVM>>(existingQualityCertificate);
       

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");


            if (TempData["QualityCertificate_EducationalFacilitiesId"] != null)
            {
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr", TempData["QualityCertificate_EducationalFacilitiesId"]);
            }
            ViewBag.existingQualityCertificate = existingQualityCertificateVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QualityCertificateVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingQualityCertificate = await _QualityCertificateService.GetAllAsync();
                var existingQualityCertificateVM = _mapper.Map<List<QualityCertificateVM>>(existingQualityCertificate);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingQualityCertificate = existingQualityCertificateVM;

                return View(model);
            }

            // Save the image
            if (model.UploadedImage != null)
            {


                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "QualityCertificateImage");

                if (relativePath != null)
                {
                    await _QualityCertificateService.AddAsync(new QualityCertificate
                    {
                        EducationalFacilitiesId = model.EducationalFacilitiesId,
                        TitleAr = model.TitleAr,
                        TitleEn = model.TitleEn,
                        IsDeleted = false,
                        IsActive = true,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today),
                        ImagePath = relativePath
                    });

                }

            }
            TempData["Success"] = "تمت الاضافة بنجاح";
            TempData["QualityCertificate_EducationalFacilitiesId"] = model.EducationalFacilitiesId;
            return RedirectToAction(nameof(Create));
            //return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var QualityCertificate = await _QualityCertificateService.GetByIdAsync(id);
            if (QualityCertificate == null) return NotFound();

            var model = _mapper.Map<QualityCertificateVM>(QualityCertificate);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QualityCertificateVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            var entity = await _QualityCertificateService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            // If no new image uploaded AND no existing image, throw validation error
            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }

           

            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
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
                    .UploadImageAsync(model.UploadedImage, "QualityCertificateImage");

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

            await _QualityCertificateService.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var QualityCertificate = await _QualityCertificateService.GetByIdAsync(id);
            if (QualityCertificate == null) return NotFound();

            await _QualityCertificateService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetCertificateByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var certificates = await _QualityCertificateService.GetAllAsync();
            certificates = certificates.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<QualityCertificateVM>>(certificates);


            return PartialView("_QualityCertificatePartial", vmList);
        }
    }
}
