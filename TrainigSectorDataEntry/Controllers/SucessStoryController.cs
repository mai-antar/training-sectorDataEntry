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
    public class SucessStoryController : Controller
    {
        private readonly IGenericService<SucessStory> _SucessStoryService;

        private readonly IGenericService<TrainingSector> _trainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public SucessStoryController(IGenericService<SucessStory> SucessStoryService, IGenericService<TrainingSector> trainingSectorService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _SucessStoryService = SucessStoryService;
            _trainingSectorService = trainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var SucessStoryList = await _SucessStoryService.GetAllAsync();

            var sectors = await _trainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
          
            var viewModelList = _mapper.Map<List<SucessStoryVM>>(SucessStoryList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var sectors = await _trainingSectorService.GetDropdownListAsync();

            var existingSucessStory = await _SucessStoryService.GetAllAsync();
            var existingSucessStoryVM = _mapper.Map<List<SucessStoryVM>>(existingSucessStory);

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");

            if (TempData["SucessStory_TrainigSectorId"] != null )
            {
                ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr", TempData["SucessStory_TrainigSectorId"]);
            }

            ViewBag.ExistingSucessStory = existingSucessStoryVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SucessStoryVM model)
        {
        
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }


            //if (model.IsExternalLink == true)
            //{
            //    if (string.IsNullOrEmpty(model.VedioPath))
            //    {
            //        ModelState.AddModelError("VedioPath", "يرجى إدخال الرابط الخارجي للفيديو.");
            //    }
            //}
            //else
            //{
            //    if (model.VedioFile == null || model.VedioFile.Length == 0)
            //    {
            //        ModelState.AddModelError("VedioFile", "يجب تحميل فيديو.");
            //    }
            //}

            if (!ModelState.IsValid)
            {
                var sectors = await _trainingSectorService.GetDropdownListAsync();
                var existingSucessStory = await _SucessStoryService.GetAllAsync();
                var existingSucessStoryVM = _mapper.Map<List<SucessStoryVM>>(existingSucessStory);

                ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
                ViewBag.ExistingSucessStory = existingSucessStoryVM;

                return View(model);
            }

        
            if (model.UploadedImage != null)
            {
                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "SucessStoryImage");

                if (relativePath != null)
                {
                    var entity = _mapper.Map<SucessStory>(model);
                    entity.IsDeleted = false;
                    entity.IsActive = true;
                    entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                    entity.ImagePath = relativePath;
                    await _SucessStoryService.AddAsync(entity);
                }


                //if (model.IsExternalLink == false)
                //{
                //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
                //    if (!Directory.Exists(uploadsFolder))
                //        Directory.CreateDirectory(uploadsFolder);

                //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.VedioFile!.FileName);
                //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await model.VedioFile.CopyToAsync(stream);
                //    }

                //    entity.VedioPath = "/videos/" + uniqueFileName;
                //}
             
            }
            TempData["Success"] = "تمت الاضافة بنجاح";
           
            TempData["SucessStory_TrainigSectorId"] = model.TrainigSectorId;
            return RedirectToAction(nameof(Create));
            //return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var SucessStory = await _SucessStoryService.GetByIdAsync(id);
            if (SucessStory == null) return NotFound();

            var model = _mapper.Map<SucessStoryVM>(SucessStory);
            var sectors = await _trainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SucessStoryVM model)
        {
            if (!ModelState.IsValid)
            {
                var sectors = await _trainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
                return View(model);
            }
          
            var entity = await _SucessStoryService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();
           

            // Update fields
            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.DescriptionEn = model.DescriptionEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.IsActive = model.IsActive;
            //entity.IsExternalLink = model.IsExternalLink;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);


            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }
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
                var sectors = await _trainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
                return View(model);
            }
            //if (model.IsExternalLink)
            //{
            //    entity.VedioPath = model.VedioPath; // External link
            //}
            //else if (model.VedioFile != null && model.VedioFile.Length > 0)
            //{
            //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            //    if (!Directory.Exists(uploadsFolder))
            //        Directory.CreateDirectory(uploadsFolder);

            //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.VedioFile.FileName);
            //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await model.VedioFile.CopyToAsync(stream);
            //    }

            //    entity.VedioPath = "/videos/" + uniqueFileName;
            //}


            await _SucessStoryService.UpdateAsync(entity);


            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }

 
        public async Task<IActionResult> Delete(int id, string returnTo)
        {
            var SucessStory = await _SucessStoryService.GetByIdAsync(id);
            if (SucessStory == null) return NotFound();

            if (SucessStory.ImagePath != null)
            {
                await _fileStorageService.DeleteFileAsync(SucessStory.ImagePath);
            }

            await _SucessStoryService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return returnTo == "Create" ? RedirectToAction(nameof(Create)) : RedirectToAction(nameof(Index));
            //return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetSucessStoryByTrainingSectorId(int trainingSectorId)
        {
           
            var sectors = await _trainingSectorService.GetDropdownListAsync();
           var SucessStory = await _SucessStoryService.GetAllAsync();
            SucessStory = SucessStory.Where(a => a.TrainigSectorId == trainingSectorId).ToList();

            var vmList = _mapper.Map<List<SucessStoryVM>>(SucessStory);


            return PartialView("_SucessStoryPartial", vmList);
        }
    }
}
