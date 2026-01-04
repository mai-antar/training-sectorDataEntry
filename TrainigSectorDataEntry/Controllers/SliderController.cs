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
            ViewBag.existingSlider = existingSliderVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var trainingSector = await _trainingSectorService.GetDropdownListAsync();
                var existingSlider = await _sliderService.GetAllAsync();
                var existingSliderVM = _mapper.Map<List<SliderVM>>(existingSlider);

                ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr");
                ViewBag.existingSlider = existingSliderVM;

                return View(model);
            }
            

            // Save the image

            if (model.UploadedImage != null)
            {


                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "SliderImage");

                if (relativePath != null)
                {
                    await _sliderService.AddAsync(new Slider
                    {

                        TrainigSectorId = model.TrainigSectorId,
                        TitleAr = model.TitleAr,
                        TitleEn = model.TitleEn,
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
            if (!ModelState.IsValid)
            {
                var trainingSector = await _trainingSectorService.GetDropdownListAsync();
                ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr");
                return View(model);
            }

            var entity = await _sliderService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            // If no new image uploaded AND no existing image, throw validation error
            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }



            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.TrainigSectorId = model.TrainigSectorId;
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
                    var educationalFacility = await _sliderService.GetDropdownListAsync();
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
                var trainingSector = await _trainingSectorService.GetDropdownListAsync();
                ViewBag.trainingSectorList = new SelectList(trainingSector, "Id", "NameAr");
                return View(model);
            }

            await _sliderService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Slider = await _sliderService.GetByIdAsync(id);
            if (Slider == null) return NotFound();

            await _sliderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
