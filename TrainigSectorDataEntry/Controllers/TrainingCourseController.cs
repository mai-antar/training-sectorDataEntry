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
    public class TrainingCourseController : Controller
    {
        private readonly IGenericService<TrainingCourse> _TrainingCourseService;
        private readonly IGenericService<TrainingCoursesType> _TrainingCoursesTypeService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;

        public TrainingCourseController(
            IGenericService<TrainingCourse> TrainingCourse,
            IGenericService<TrainingCoursesType> TrainingCoursesTypeService,
            IMapper mapper,
            ILoggerRepository logger,
            IFileStorageService fileStorageService)
        {
            _TrainingCourseService = TrainingCourse;
            _TrainingCoursesTypeService = TrainingCoursesTypeService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var TrainingCourseList = await _TrainingCourseService.GetAllAsync();
            var TrainingCoursesType = await _TrainingCoursesTypeService.GetDropdownListAsync();

            ViewBag.TrainingCoursesTypeList = new SelectList(TrainingCoursesType, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<TrainingCourseVM>>(TrainingCourseList);
            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingCoursesType = await _TrainingCoursesTypeService.GetDropdownListAsync();
            var existingTrainingCourse = await _TrainingCourseService.GetAllAsync();
            var existingTrainingCourseVM = _mapper.Map<List<TrainingCourseVM>>(existingTrainingCourse);

            ViewBag.TrainingCoursesTypeList = new SelectList(TrainingCoursesType, "Id", "NameAr");
            ViewBag.existingTrainingCourse = existingTrainingCourseVM;

            return View();
        }

        // ========================= CREATE POST =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingCourseVM model)
        {
            //  Arabic file is REQUIRED
            if (model.UploadedFileAr == null || model.UploadedFileAr.Length == 0)
            {
                ModelState.AddModelError("UploadedFileAr", "يجب تحميل الملف العربي");
            }

            if (!ModelState.IsValid)
            {
                var TrainingCoursesType = await _TrainingCoursesTypeService.GetDropdownListAsync();
                var existingTrainingCourse = await _TrainingCourseService.GetAllAsync();
                var existingTrainingCourseVM = _mapper.Map<List<TrainingCourseVM>>(existingTrainingCourse);

                ViewBag.TrainingCoursesTypeList = new SelectList(TrainingCoursesType, "Id", "NameAr");
                ViewBag.existingTrainingCourse = existingTrainingCourseVM;

                return View(model);
            }

            string[] allowedDocs = { ".pdf", ".docx", ".xlsx" };

            string? arPath = null;
            string? enPath = null;

            //  Upload Arabic file (required)
            if (model.UploadedFileAr != null)
            {
                arPath = await _fileStorageService
                    .UploadFileAsync(model.UploadedFileAr, "TrainingCourse/Ar", allowedDocs);
            }

            //  Upload English file (optional)
            if (model.UploadedFileEn != null)
            {
                enPath = await _fileStorageService
                    .UploadFileAsync(model.UploadedFileEn, "TrainingCourse/En", allowedDocs);
            }

            var entity = _mapper.Map<TrainingCourse>(model);

            entity.FilePathAr = arPath;
            entity.FilePathEn = enPath;
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

            await _TrainingCourseService.AddAsync(entity);

            TempData["Success"] = "تمت الاضافة بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // ========================= EDIT GET =========================
        public async Task<IActionResult> Edit(int id)
        {
            var TrainingCourse = await _TrainingCourseService.GetByIdAsync(id);
            if (TrainingCourse == null) return NotFound();

            var model = _mapper.Map<TrainingCourseVM>(TrainingCourse);
            var TrainingCoursesType = await _TrainingCoursesTypeService.GetDropdownListAsync();

            ViewBag.TrainingCoursesTypeList = new SelectList(TrainingCoursesType, "Id", "NameAr");
            return View(model);
        }

        // ========================= EDIT POST =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TrainingCourseVM model)
        {
            if (!ModelState.IsValid)
            {
                var TrainingCoursesType = await _TrainingCoursesTypeService.GetDropdownListAsync();
                ViewBag.TrainingCoursesTypeList = new SelectList(TrainingCoursesType, "Id", "NameAr");
                return View(model);
            }

            var entity = await _TrainingCourseService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            //  Arabic file must exist (old or new)
            if (model.UploadedFileAr == null && string.IsNullOrEmpty(entity.FilePathAr))
            {
                ModelState.AddModelError("UploadedFileAr", "يجب تحميل الملف العربي");
                return View(model);
            }

            string[] allowedDocs = { ".pdf", ".docx", ".xlsx" };

            //  Update Arabic file if uploaded
            if (model.UploadedFileAr != null)
            {
                if (!string.IsNullOrEmpty(entity.FilePathAr))
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePathAr);
                }

                entity.FilePathAr = await _fileStorageService
                    .UploadFileAsync(model.UploadedFileAr, "TrainingCourse/Ar", allowedDocs);
            }

            //  Update English file if uploaded (optional)
            if (model.UploadedFileEn != null)
            {
                if (!string.IsNullOrEmpty(entity.FilePathEn))
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePathEn);
                }

                entity.FilePathEn = await _fileStorageService
                    .UploadFileAsync(model.UploadedFileEn, "TrainingCourse/En", allowedDocs);
            }

            entity.NameAr = model.NameAr;
            entity.NameEn = model.NameEn;
            entity.TrainigCoursesTypesId = model.TrainigCoursesTypesId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

            await _TrainingCourseService.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // ========================= DELETE =========================
        public async Task<IActionResult> Delete(int id, string returnTo)
        {
            var TrainingCourse = await _TrainingCourseService.GetByIdAsync(id);
            if (TrainingCourse == null) return NotFound();

            if (!string.IsNullOrEmpty(TrainingCourse.FilePathAr))
                await _fileStorageService.DeleteFileAsync(TrainingCourse.FilePathAr);

            if (!string.IsNullOrEmpty(TrainingCourse.FilePathEn))
                await _fileStorageService.DeleteFileAsync(TrainingCourse.FilePathEn);

            await _TrainingCourseService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return returnTo == "Create"
                ? RedirectToAction(nameof(Create))
                : RedirectToAction(nameof(Index));
        }

        // ========================= FILTER =========================
        [HttpGet]
        public async Task<IActionResult> GetTrainingCourseByTrainingCourseType(int trainigCoursesTypesId)
        {
            var trainingCourses = await _TrainingCourseService.GetAllAsync();
            trainingCourses = trainingCourses
                .Where(a => a.TrainigCoursesTypesId == trainigCoursesTypesId)
                .ToList();

            var vmList = _mapper.Map<List<TrainingCourseVM>>(trainingCourses);
            return PartialView("_TrainingCoursePartial", vmList);
        }
    }
}

