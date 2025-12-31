using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class AlertsAndAdvertismentController : Controller
    {

        private readonly IGenericService<AlertsAndAdvertisment> _AlertsAndAdvertismentServices;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public AlertsAndAdvertismentController(IGenericService<AlertsAndAdvertisment> AlertsAndAdvertismentServices,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _AlertsAndAdvertismentServices = AlertsAndAdvertismentServices;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var AlertsAndAdvertismentList = await _AlertsAndAdvertismentServices.GetAllAsync();
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<AlertsAndAdvertismentVM>>(AlertsAndAdvertismentList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create(int educationalFacilityId=0)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingAlertsAndAdvertisment = await _AlertsAndAdvertismentServices.GetAllAsync();
            var existingAlertsAndAdvertismentVM = _mapper.Map<List<AlertsAndAdvertismentVM>>(existingAlertsAndAdvertisment);



            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.existingAlertsAndAdvertisment = existingAlertsAndAdvertismentVM;
            return View(new AlertsAndAdvertismentVM
            {
                EducationalFacilitiesId = educationalFacilityId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlertsAndAdvertismentVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingAlertsAndAdvertisment = await _AlertsAndAdvertismentServices.GetAllAsync();
                var existingAlertsAndAdvertismentVM = _mapper.Map<List<AlertsAndAdvertismentVM>>(existingAlertsAndAdvertisment);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingAlertsAndAdvertisment = existingAlertsAndAdvertismentVM;

                return View(model);
            }

            if (model.UploadedImage != null )
            {

               
                    var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "historyBreifImage");

                    if (relativePath != null)
                    {
                        await _AlertsAndAdvertismentServices.AddAsync(new AlertsAndAdvertisment
                        {
                               EducationalFacilitiesId=model.EducationalFacilitiesId,
                           IsDeleted = false,
                           IsActive = true,
                           UserCreationDate = DateOnly.FromDateTime(DateTime.Today),
                           ImagePath = relativePath
                        });

                    }
                
            }
          

            return View(new AlertsAndAdvertismentVM
            {
                EducationalFacilitiesId = model.EducationalFacilitiesId
            });

         
        }


        public async Task<IActionResult> Edit(int id)
        {
            var AlertsAndAdvertisment = await _AlertsAndAdvertismentServices.GetByIdAsync(id);
            if (AlertsAndAdvertisment == null) return NotFound();

            var model = _mapper.Map<AlertsAndAdvertismentVM>(AlertsAndAdvertisment);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AlertsAndAdvertismentVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList =
                    new SelectList(educationalFacility, "Id", "NameAr");

                return View(model);
            }

            var entity = await _AlertsAndAdvertismentServices.GetByIdAsync(model.Id);
            if (entity == null)
                return NotFound();

            
            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList =
                    new SelectList(educationalFacility, "Id", "NameAr");
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
                    .UploadImageAsync(model.UploadedImage, "AlertsAndAdvertismentImage");

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

            await _AlertsAndAdvertismentServices.UpdateAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var AlertsAndAdvertisment = await _AlertsAndAdvertismentServices.GetByIdAsync(id);
            if (AlertsAndAdvertisment == null) return NotFound();

            await _AlertsAndAdvertismentServices.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAlertsByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var alerts = await _AlertsAndAdvertismentServices.GetAllAsync();
            alerts = alerts.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<AlertsAndAdvertismentVM>>(alerts);

           
            return PartialView("_AlertsAndAdvertismentPartial", vmList);
        }

    }
}
