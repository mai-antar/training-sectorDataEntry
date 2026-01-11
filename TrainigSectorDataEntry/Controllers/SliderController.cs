using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class SliderController : Controller
    {
        private readonly IGenericService<Slider> _sliderService;
        private readonly IGenericService<TrainingSector> _trainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;

        public SliderController(IGenericService<Slider> sliderService,
            IGenericService<TrainingSector> trainingSectorService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _sliderService = sliderService;
            _trainingSectorService = trainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var SliderList = await _sliderService.GetAllAsync();
            var trainingSector = await _trainingSectorService.GetDropdownListAsync();

            ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<SliderVM>>(SliderList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var trainingSector = await _trainingSectorService.GetDropdownListAsync();

            var existingSlider = await _sliderService.GetAllAsync();
            var existingSliderVM = _mapper.Map<List<SliderVM>>(existingSlider);



            ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr");

            if (TempData["SelectedSectorId"] != null)
            {
                ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr", TempData["SelectedSectorId"]);

            }
            ViewBag.existingSlider = existingSliderVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderVM model)
        {
         
            if (model.IsVideo == false)
            {
                if (model.UploadedFile == null || model.UploadedFile.Length == 0)
                    ModelState.AddModelError("UploadedFile", "يجب تحميل صورة.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.FilePath))
                    ModelState.AddModelError("FilePath", "يرجى إدخال رابط الفيديو.");
            }

            if (!ModelState.IsValid)
            {
                var trainingSector = await _trainingSectorService.GetDropdownListAsync();
                var existingSlider = await _sliderService.GetAllAsync();

                ViewBag.trainingSectorList =
                    new SelectList(trainingSector, "Id", "NameAr");

                ViewBag.existingSlider =
                    _mapper.Map<List<SliderVM>>(existingSlider);

                return View(model);
            }

         
            string filePath;

            if (model.IsVideo == false)
            {
                filePath = await _fileStorageService
                    .UploadImageAsync(model.UploadedFile, "SliderImage");
            }
            else
            {
                filePath = model.FilePath;
            }

            await _sliderService.AddAsync(new Slider
            {
                TrainigSectorId = model.TrainigSectorId,
                TitleAr = model.TitleAr,
                TitleEn = model.TitleEn,
                DescriptionAr = model.DescriptionAr,
                DescriptionEn = model.DescriptionEn,
                IsVideo = model.IsVideo,
                IsActive = true,
                IsDeleted = false,
                UserCreationDate = DateOnly.FromDateTime(DateTime.Today),
                FilePath = filePath
            });

            TempData["Success"] = "تمت الاضافة بنجاح";
            TempData["SelectedSectorId"] = model.TrainigSectorId;

            return RedirectToAction(nameof(Create));
        }



        public async Task<IActionResult> Edit(int id)
        {
            var Slider = await _sliderService.GetByIdAsync(id);
            if (Slider == null) return NotFound();

            var model = _mapper.Map<SliderVM>(Slider);
            var trainingSector = await _trainingSectorService.GetDropdownListAsync();
            ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderVM model)
        {
            var entity = await _sliderService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();


            if (model.IsVideo == true)
            {
                if (string.IsNullOrWhiteSpace(model.FilePath))
                    ModelState.AddModelError("FilePath", "يرجى إدخال رابط الفيديو.");
            }
            else
            {
                if (model.UploadedFile == null && string.IsNullOrEmpty(entity.FilePath))
                    ModelState.AddModelError("UploadedFile", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var trainingSector = await _trainingSectorService.GetDropdownListAsync();
                ViewBag.trainingSectorList =
                    new SelectList(trainingSector, "Id", "NameAr");

                return View(model);
            }

         
            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.IsActive = model.IsActive;
            entity.IsVideo = model.IsVideo;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

            
            if (model.IsVideo == true)
            {
                // delete old image if exists
                if (!string.IsNullOrEmpty(entity.FilePath) &&
                    !entity.FilePath.StartsWith("http"))
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePath);
                }

                entity.FilePath = model.FilePath;
            }
          
            else if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(entity.FilePath))
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePath);
                }

                entity.FilePath = await _fileStorageService
                    .UploadImageAsync(model.UploadedFile, "SliderImage");
            }

            await _sliderService.UpdateAsync(entity);
            TempData["Success"] = "تم التعديل بنجاح";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var Slider = await _sliderService.GetByIdAsync(id);
            if (Slider == null) return NotFound();

            await _sliderService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
