
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class StudentsTimeTableController : Controller
    {

        private readonly IGenericService<StudentsTimeTable> _StudentsTimeTableService;
        private readonly IGenericService<EducationalLevel> _EducationalLevelService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;
        public StudentsTimeTableController(IGenericService<StudentsTimeTable> StudentsTimeTableService,
            IGenericService<EducationalLevel> EducationalLevelService, IGenericService<EducationalFacility> educationalFacilityService,
            IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _StudentsTimeTableService = StudentsTimeTableService;
            _EducationalLevelService = EducationalLevelService;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<IActionResult> Index()
        {
            var StudentsTimeTableList = await _StudentsTimeTableService.GetAllAsync();


            var educationalLevel = await _EducationalLevelService.GetDropdownListAsync();
            ViewBag.EducationalLevelList = new SelectList(educationalLevel, "Id", "NameAr");

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<StudentsTimeTableVM>>(StudentsTimeTableList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            //var EducationalLevel = await _EducationalLevelService.GetDropdownListAsync();

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var existingStudentsTimeTable = await _StudentsTimeTableService.GetAllAsync();
            var existingStudentsTimeTableVM = _mapper.Map<List<StudentsTimeTableVM>>(existingStudentsTimeTable);


            ViewBag.EducationalLevelList = new List<SelectListItem>();
            //ViewBag.EducationalLevelList = new SelectList(EducationalLevel, "Id", "NameAr");
            ViewBag.ExistingStudentsTimeTable = existingStudentsTimeTableVM;
            return View();
        }
 


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentsTimeTableVM model)
        {
            // Validate that an file is uploaded
            if (model.UploadedFile == null || model.UploadedFile.Length == 0)
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
            }

            if (!ModelState.IsValid)
            {
                var EducationalLevel = await _EducationalLevelService.GetDropdownListAsync();


                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");


                var existingStudentsTimeTable = await _StudentsTimeTableService.GetAllAsync();
                var existingStudentsTimeTableVM = _mapper.Map<List<StudentsTimeTableVM>>(existingStudentsTimeTable);

                ViewBag.EducationalLevelList = new SelectList(EducationalLevel, "Id", "NameAr");
                ViewBag.ExistingStudentsTimeTable = existingStudentsTimeTableVM;

                return View(model);
            }
            // Save the file
            string? fileName = null;
            if (model.UploadedFile != null)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/StudentsTimeTableFile");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UploadedFile.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedFile.CopyToAsync(stream);
                    }
                }
            }

            var entity = _mapper.Map<StudentsTimeTable>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
            entity.FilePath = "/uploads/StudentsTimeTableFile/" + fileName;
            await _StudentsTimeTableService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var StudentsTimeTable = await _StudentsTimeTableService.GetByIdAsync(id);
            if (StudentsTimeTable == null) return NotFound();

            var model = _mapper.Map<StudentsTimeTableVM>(StudentsTimeTable);

            var EducationalLevel = await _EducationalLevelService.GetDropdownListAsync();
            var selectedEducationalLevel = EducationalLevel.FirstOrDefault(a => a.Id == StudentsTimeTable.EducationalLevelId);
            int EducationalFacilitiesId = selectedEducationalLevel?.EducationalFacilitiesId ?? 0;

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var educationalFacilityLevels= EducationalLevel.Where(a => a.EducationalFacilitiesId== EducationalFacilitiesId).ToList();

            model.EducationalFacilitiesId = EducationalFacilitiesId;
            ViewBag.EducationalLevelList = new SelectList(educationalFacilityLevels, "Id", "NameAr");
            ViewBag.EducationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.SelectedFacilityId = EducationalFacilitiesId;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentsTimeTableVM model)
        {
            if (!ModelState.IsValid)
            {
                var EducationalLevel = await _EducationalLevelService.GetDropdownListAsync();
                ViewBag.EducationalLevelList = new SelectList(EducationalLevel, "Id", "NameAr");
                return View(model);
            }

            var entity = await _StudentsTimeTableService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

     
            if (model.UploadedFile == null && string.IsNullOrEmpty(entity.FilePath))
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
                return View(model);
            }



       
            entity.IsActive = model.IsActive;
            entity.IsCurrent = model.IsCurrent;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);


            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                // Delete old file if exists
                if (!string.IsNullOrEmpty(entity.FilePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entity.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new file
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/StudentsTimeTableFile");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UploadedFile.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(stream);
                }

                // Update entity path
                entity.FilePath = "/uploads/StudentsTimeTableFile/" + fileName;
            }

            // Ensure file path is still set
            if (string.IsNullOrEmpty(entity.FilePath))
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
                var EducationalLevel = await _EducationalLevelService.GetDropdownListAsync();
                ViewBag.EducationalLevelList = new SelectList(EducationalLevel, "Id", "NameAr");
                return View(model);
            }

            await _StudentsTimeTableService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var StudentsTimeTable = await _StudentsTimeTableService.GetByIdAsync(id);
            if (StudentsTimeTable == null) return NotFound();

            await _StudentsTimeTableService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<JsonResult> GetLevelsByFacilityId(int facilityId)
        {
            var levels = await _EducationalLevelService.GetAllAsync();
            var filteredLevels = levels
                .Where(x => x.EducationalFacilitiesId == facilityId)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NameAr
                })
                .ToList();

            return Json(filteredLevels);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsTimeTableByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var studentsTimeTable = await _StudentsTimeTableService.GetAllAsync();
            studentsTimeTable = studentsTimeTable.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<StudentsTimeTableVM>>(studentsTimeTable);


            return PartialView("_AlertsAndAdvertismentPartial", vmList);
        }


    }
}
