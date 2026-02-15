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
  
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public ServiceController(IGenericService<Service> Services,
             IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _Services = Services;

            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var ServiceList = await _Services.GetAllAsync();
        

            var viewModelList = _mapper.Map<List<ServiceVM>>(ServiceList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
        

            var existingService = await _Services.GetAllAsync();
            var existingServiceVM = _mapper.Map<List<ServiceVM>>(existingService);



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
         
                var existingService = await _Services.GetAllAsync();
                var existingServiceVM = _mapper.Map<List<ServiceVM>>(existingService);

               
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
            TempData["Success"] = "تمت الاضافة بنجاح";

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var Service = await _Services.GetByIdAsync(id);
            if (Service == null) return NotFound();

            var model = _mapper.Map<ServiceVM>(Service);
     
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceVM model)
        {
        
            var entity = await _Services.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();


            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }



            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.Type = model.Type;
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
       
                return View(model);
            }

            await _Services.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";

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

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }

    }
}
