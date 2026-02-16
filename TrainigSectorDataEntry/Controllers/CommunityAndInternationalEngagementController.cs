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
    public class CommunityAndInternationalEngagementController : Controller
    {
        private readonly IGenericService<CommunityAndInternationalEngagement> _CommunityAndInternationalEngagement;
  
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public CommunityAndInternationalEngagementController(IGenericService<CommunityAndInternationalEngagement> CommunityAndInternationalEngagement,
             IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _CommunityAndInternationalEngagement = CommunityAndInternationalEngagement;

            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var CommunityAndInternationalEngagementList = await _CommunityAndInternationalEngagement.GetAllAsync();
        

            var viewModelList = _mapper.Map<List<CommunityAndInternationalEngagementVM>>(CommunityAndInternationalEngagementList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
        

            var existingCommunityAndInternationalEngagement = await _CommunityAndInternationalEngagement.GetAllAsync();
            var existingCommunityAndInternationalEngagementVM = _mapper.Map<List<CommunityAndInternationalEngagementVM>>(existingCommunityAndInternationalEngagement);



            ViewBag.existingCommunityAndInternationalEngagement = existingCommunityAndInternationalEngagementVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommunityAndInternationalEngagementVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
         
                var existingCommunityAndInternationalEngagement = await _CommunityAndInternationalEngagement.GetAllAsync();
                var existingCommunityAndInternationalEngagementVM = _mapper.Map<List<CommunityAndInternationalEngagementVM>>(existingCommunityAndInternationalEngagement);

               
                ViewBag.existingCommunityAndInternationalEngagement = existingCommunityAndInternationalEngagementVM;

                return View(model);
            }

            // Save the image
            if (model.UploadedImage != null)
            {
                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "ServiceImage");

                if (relativePath != null)
                {
                    // Map and save the entity
                    var entity = _mapper.Map<CommunityAndInternationalEngagement>(model);
                    entity.IsDeleted = false;
                    entity.IsActive = true;
                    entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                    entity.ImagePath = relativePath;

                    await _CommunityAndInternationalEngagement.AddAsync(entity);
                }
         
            }

            TempData["Success"] = "تمت الاضافة بنجاح";
        
            return RedirectToAction(nameof(Create));
            //TempData["Success"] = "تمت الاضافة بنجاح";

            //return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var CommunityAndInternationalEngagement = await _CommunityAndInternationalEngagement.GetByIdAsync(id);
            if (CommunityAndInternationalEngagement == null) return NotFound();

            var model = _mapper.Map<CommunityAndInternationalEngagementVM>(CommunityAndInternationalEngagement);
     
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CommunityAndInternationalEngagementVM model)
        {
        
            var entity = await _CommunityAndInternationalEngagement.GetByIdAsync(model.Id);
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

            await _CommunityAndInternationalEngagement.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        { 
            var CommunityAndInternationalEngagement = await _CommunityAndInternationalEngagement.GetByIdAsync(id);
            if (CommunityAndInternationalEngagement == null) return NotFound();

            if (CommunityAndInternationalEngagement.ImagePath != null)
            {
                await _fileStorageService.DeleteFileAsync(CommunityAndInternationalEngagement.ImagePath);
            }

            await _CommunityAndInternationalEngagement.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }

     

    }
}
