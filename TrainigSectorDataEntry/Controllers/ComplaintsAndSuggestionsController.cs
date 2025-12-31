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
    public class ComplaintsAndSuggestionsController : Controller
    {
        private readonly IGenericService<ComplaintsAndSuggestion> _ComplaintsAndSuggestionService;
        private readonly IGenericService<TrainingSector> _TrainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public ComplaintsAndSuggestionsController(IGenericService<ComplaintsAndSuggestion> ComplaintsAndSuggestion,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _ComplaintsAndSuggestionService = ComplaintsAndSuggestion;
            _TrainingSectorService = TrainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var ComplaintsAndSuggestionList = await _ComplaintsAndSuggestionService.GetAllAsync();
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<ComplaintsAndSuggestionVM>>(ComplaintsAndSuggestionList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            var existingComplaintsAndSuggestion = await _ComplaintsAndSuggestionService.GetAllAsync();
            var existingComplaintsAndSuggestionVM = _mapper.Map<List<ComplaintsAndSuggestionVM>>(existingComplaintsAndSuggestion);



            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            ViewBag.existingComplaintsAndSuggestion = existingComplaintsAndSuggestionVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintsAndSuggestionVM model)
        {
            string[] allowedDocs = { ".pdf", ".docx", ".xlsx" };

            if (model.UploadedFile == null || model.UploadedFile.Length == 0)
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
            }
            else
            {
                var extension = Path.GetExtension(model.UploadedFile.FileName).ToLowerInvariant();
                if (!allowedDocs.Contains(extension))
                {
                    ModelState.AddModelError(
                        "UploadedFile",
                        "صيغة الملف غير مدعومة. الصيغ المسموحة: pdf, docx, xlsx"
                    );
                }
            }


            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                var existingComplaintsAndSuggestion = await _ComplaintsAndSuggestionService.GetAllAsync();
                var existingComplaintsAndSuggestionVM = _mapper.Map<List<ComplaintsAndSuggestionVM>>(existingComplaintsAndSuggestion);

                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                ViewBag.existingComplaintsAndSuggestion = existingComplaintsAndSuggestionVM;

                return View(model);
            }

            if (model.UploadedFile != null)
            {
               
                var relativePath = await _fileStorageService.UploadFileAsync(model.UploadedFile, "ComplaintsAndSuggestion", allowedDocs);

           

                // Map and save the entity
                var entity = _mapper.Map<ComplaintsAndSuggestion>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                entity.FilePath = relativePath;

                await _ComplaintsAndSuggestionService.AddAsync(entity);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var ComplaintsAndSuggestion = await _ComplaintsAndSuggestionService.GetByIdAsync(id);
            if (ComplaintsAndSuggestion == null) return NotFound();

            var model = _mapper.Map<ComplaintsAndSuggestionVM>(ComplaintsAndSuggestion);
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ComplaintsAndSuggestionVM model)
        {
            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                return View(model);
            }
       

            var entity = await _ComplaintsAndSuggestionService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            if (model.UploadedFile == null && string.IsNullOrEmpty(entity.FilePath))
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
                return View(model);
            }


            entity.Name = model.Name;
            entity.Telephone = model.Telephone;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.ComplaintText = model.ComplaintText;
            entity.Email = model.Email;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                // Delete old file if exists
                if (!string.IsNullOrEmpty(entity.FilePath))
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePath);


                }
                string[] allowedDocs = { ".pdf", ".docx", ".xlsx" };
                var relativePath = await _fileStorageService.UploadFileAsync(model.UploadedFile, "ComplaintsAndSuggestion", allowedDocs);

                // Update entity path
                entity.FilePath = relativePath;
            }

            await _ComplaintsAndSuggestionService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ComplaintsAndSuggestion = await _ComplaintsAndSuggestionService.GetByIdAsync(id);
            if (ComplaintsAndSuggestion == null) return NotFound();

            await _ComplaintsAndSuggestionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetComplaintsByTrainingSector(int trainigSectorId)
        {
            var educationalFacility = await _TrainingSectorService.GetDropdownListAsync();
            var alerts = await _ComplaintsAndSuggestionService.GetAllAsync();
            alerts = alerts.Where(a => a.TrainigSectorId == trainigSectorId).ToList();

            var vmList = _mapper.Map<List<ComplaintsAndSuggestionVM>>(alerts);


            return PartialView("_ComplaintsAndSuggestionsPartial", vmList);
        }
    }
}
