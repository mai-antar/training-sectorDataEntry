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
    public class ServiceController : Controller
    {
        private readonly IGenericService<Service> _Services;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public ServiceController(IGenericService<Service> Services,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _Services = Services;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var ServiceList = await _Services.GetAllAsync();
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<ServiceVM>>(ServiceList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingService = await _Services.GetAllAsync();
            var existingServiceVM = _mapper.Map<List<ServiceVM>>(existingService);



            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.existingService = existingServiceVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingService = await _Services.GetAllAsync();
                var existingServiceVM = _mapper.Map<List<ServiceVM>>(existingService);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingService = existingServiceVM;

                return View(model);
            }

            // Save the image
            if (model.UploadedImage != null)
            {
                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "ServiceImage");

                if (relativePath != null)
                {
                    // Map and save the entity
                    var entity = _mapper.Map<Service>(model);
                    entity.IsDeleted = false;
                    entity.IsActive = true;
                    entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                    entity.ImagePath = relativePath;

                    await _Services.AddAsync(entity);
                }
         
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var Service = await _Services.GetByIdAsync(id);
            if (Service == null) return NotFound();

            var model = _mapper.Map<ServiceVM>(Service);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            var entity = await _Services.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();


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
                // Delete old image if exists
                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(entity.ImagePath);

                }
                // Save new image
                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "SucessStoryImage");

                if (relativePath != null)
                {
                    // Update entity path
                    entity.ImagePath = relativePath;
                }
        
            }

            // Ensure image path is still set
            if (string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            await _Services.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        { 
            var Service = await _Services.GetByIdAsync(id);
            if (Service == null) return NotFound();

            if (Service.ImagePath != null)
            {
                await _fileStorageService.DeleteFileAsync(Service.ImagePath);
            }

            await _Services.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
