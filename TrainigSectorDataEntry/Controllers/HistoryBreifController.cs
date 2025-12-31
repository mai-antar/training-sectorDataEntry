using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;
namespace TrainigSectorDataEntry.Controllers
{
    public class HistoryBreifController : Controller
    {
        private readonly IGenericService<HistoryBreif> _historyBreifService;
        private readonly IGenericService<HistoryBerifImage> _historyBerifImageService;

        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public HistoryBreifController(IGenericService<HistoryBreif> historyBreifService, IGenericService<HistoryBerifImage> historyBerifImageService, 
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _historyBreifService = historyBreifService;
            _historyBerifImageService = historyBerifImageService;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var historyBreifList = await _historyBreifService.GetAllAsync();
            var historyBerifImageList = await _historyBerifImageService.GetAllAsync();

            var sectors = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
            foreach (var item in historyBreifList)
            {
                if (historyBerifImageList.Where(a => a.HistoryBreifId == item.Id).ToList().Count > 0)
                {

                    item.HistoryBerifImages = historyBerifImageList.Where(a => a.HistoryBreifId == item.Id).ToList();
                }
            }

            var viewModelList = _mapper.Map<List<HistoryBreifVM>>(historyBreifList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingHistoryBreif = await _historyBreifService.GetAllAsync();
            var existingHistoryBreifVM = _mapper.Map<List<HistoryBreifVM>>(existingHistoryBreif);

            var historyBerifImageList = await _historyBerifImageService.GetAllAsync();
            foreach (var item in existingHistoryBreifVM)
            {
                if (historyBerifImageList.Where(a => a.HistoryBreifId == item.Id).ToList().Count > 0)
                {

                    item.HistoryBerifImages = historyBerifImageList.Where(a => a.HistoryBreifId == item.Id).ToList();
                }
            }

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.ExistingHistoryBreif = existingHistoryBreifVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HistoryBreifVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

                var existingHistoryBreif = await _historyBreifService.GetAllAsync();
                var existingHistoryBreifVM = _mapper.Map<List<HistoryBreifVM>>(existingHistoryBreif);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.ExistingHistoryBreif = existingHistoryBreifVM;

                return View(model);
            }


            var entity = _mapper.Map<HistoryBreif>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

            await _historyBreifService.AddAsync(entity);

            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
               

                foreach (var file in model.UploadedImages)
                {
                    var relativePath = await _fileStorageService.UploadImageAsync(file, "historyBreifImage");

                    if (relativePath != null)
                    {
                        await _historyBerifImageService.AddAsync(new HistoryBerifImage
                        {
                            HistoryBreifId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });
          
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var HistoryBreif = await _historyBreifService.GetByIdAsync(id, n => ((HistoryBreif)n).HistoryBerifImages);
           
            if (HistoryBreif == null) return NotFound();

            var model = _mapper.Map<HistoryBreifVM>(HistoryBreif);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HistoryBreifVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }


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

 
            if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
            {
                foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
                {
                    var image = await _historyBerifImageService.GetByIdAsync(id);
                    if (image != null)
                    {
                        await _fileStorageService.DeleteFileAsync(image.ImagePath);
                        await _historyBerifImageService.DeleteAsync(id);
                        //image.IsDeleted = true;
                        //await _historyBerifImageService.UpdateAsync(image);
                    }
                }
            }

            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
                foreach (var image in model.UploadedImages)
                {

                    var relativePath = await _fileStorageService.UploadImageAsync(image, "historyBreifImage");

                    if (relativePath != null)
                    {
                        await _historyBerifImageService.AddAsync(new HistoryBerifImage
                        {
                            HistoryBreifId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });

                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> AddImages(int id)
        {
            var historyBerifImage = await _historyBreifService.GetByIdAsync(id);
            if (historyBerifImage == null)
                return NotFound();

            var vm = new HistoryBerifImageVM
            {
                HistoryBreifId = id,
                Name=historyBerifImage.NameAr
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(HistoryBerifImageVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var news = await _historyBreifService.GetByIdAsync(model.HistoryBreifId);
            if (news == null)
                return NotFound();


            foreach (var image in model.UploadedImages)
            {
                var relativePath = await _fileStorageService.UploadImageAsync(image, "HistoryBreifImages");

                if (relativePath != null)
                {
                    await _historyBerifImageService.AddAsync(new HistoryBerifImage
                    {
                        HistoryBreifId = model.Id,
                        ImagePath = relativePath,
                        IsActive = true,
                        IsDeleted = false,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                    });
                }
           
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _historyBerifImageService.GetByIdAsync(id);
            if (image == null) return NotFound();

            await _fileStorageService.DeleteFileAsync(image.ImagePath);
            await _historyBerifImageService.DeleteAsync(id);
            return RedirectToAction("Edit", new { id = image.HistoryBreifId });
        }
        public async Task<IActionResult> Delete(int id)
        {
            var historyBreif = await _historyBreifService.GetByIdAsync(id);
            if (historyBreif == null) return NotFound();

            // Delete associated images from file system
            if (historyBreif.HistoryBerifImages != null && historyBreif.HistoryBerifImages.Any())
            {
                foreach (var img in historyBreif.HistoryBerifImages)
                    await _fileStorageService.DeleteFileAsync(img.ImagePath);
            }

            await _historyBreifService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetHistoryBreifByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var historyBreif = await _historyBreifService.GetAllAsync(false, x => x.EducationalFacilities,
                  x => x.HistoryBerifImages);

            historyBreif = historyBreif.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<HistoryBreifVM>>(historyBreif);


            return PartialView("_HistoryBreifPartial", vmList);
        }

    }
}
