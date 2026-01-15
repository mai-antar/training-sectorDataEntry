
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class StudentTablesAttachmentController : Controller
    {

        private readonly IGenericService<StudentTablesAttachment> _StudentTablesAttachmentService;
        private readonly IGenericService<EducationalLevel> _EducationalLevelService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IGenericService<Departmentsandbranch> _departmentsandbranchService;
        private readonly IGenericService<TableType> _tableTypeService;
        private readonly IGenericService<Term> _termService;
        private readonly IGenericService<Specialization> _specializationService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IFileStorageService _fileStorageService;

        public StudentTablesAttachmentController(IGenericService<StudentTablesAttachment> StudentTablesAttachmentService,
            IGenericService<EducationalLevel> EducationalLevelService, IGenericService<EducationalFacility> educationalFacilityService,
            IGenericService<Departmentsandbranch> departmentsandbranchService, IGenericService<TableType> tableTypeService,
            IGenericService<Term> termService, IGenericService<Specialization> specializationService,
            IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _StudentTablesAttachmentService = StudentTablesAttachmentService;
            _EducationalLevelService = EducationalLevelService;
            _educationalFacilityService = educationalFacilityService;
            _departmentsandbranchService = departmentsandbranchService;
            _tableTypeService = tableTypeService;
            _termService = termService;
            _specializationService = specializationService;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }


        public async Task<IActionResult> Index(int studentTablesAttachmentId)
        {
            var studentTablesAttachment = await _StudentTablesAttachmentService.GetByIdAsync(
                studentTablesAttachmentId, x => x.EducationalLevel, x => x.EducationalLevel.EducationalFacilities, x => x.Departmentsandbranches,
                x => x.TableType, x => x.Terms, x => x.Specialization );

            var studentTablesAttachmentList = await _StudentTablesAttachmentService.GetAllAsync(
              false, x => x.EducationalLevel, x => x.EducationalLevel.EducationalFacilities, x => x.Departmentsandbranches,
              x => x.TableType, x => x.Terms, x => x.Specialization);
 

            var viewModelList = _mapper.Map<List<StudentTablesAttachmentVM>>(studentTablesAttachmentList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {

            var existingStudentTablesAttachment = await _StudentTablesAttachmentService.GetAllAsync(
              false, x => x.EducationalLevel, x => x.EducationalLevel.EducationalFacilities, x => x.Departmentsandbranches,
              x => x.TableType, x => x.Terms, x => x.Specialization);

            var existingStudentTablesAttachmentVM = _mapper.Map<List<StudentTablesAttachmentVM>>(existingStudentTablesAttachment);


            ViewBag.EducationalLevelList = new List<SelectListItem>();
            ViewBag.ExistingStudentTablesAttachment = existingStudentTablesAttachmentVM;

            var model = new StudentTablesAttachmentVM();
            await RefillViewBags(model);

            return View(model);
        }
 


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentTablesAttachmentVM model)
        {
            // Validate that an file is uploaded
            if (model.UploadedFile == null || model.UploadedFile.Length == 0)
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
            }

            if (!ModelState.IsValid)
            {

                TempData["Error"] = "تأكد من إدخال البيانات بشكل صحيح";

            

                await RefillViewBags(model);

                var existingStudentTablesAttachment = await _StudentTablesAttachmentService.GetAllAsync();
                var existingStudentTablesAttachmentVM = _mapper.Map<List<StudentTablesAttachmentVM>>(existingStudentTablesAttachment);

              
                ViewBag.ExistingStudentTablesAttachment = existingStudentTablesAttachmentVM;

                return View(model);
            }

            // Save the file

            if (model.UploadedFile != null)
            {
                string[] allowedDocs = { ".pdf", ".docx", ".xlsx" };
                var relativePath = await _fileStorageService.UploadFileAsync(model.UploadedFile, "StudentTablesAttachmentFile", allowedDocs);


               
                 var entity = _mapper.Map<StudentTablesAttachment>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                entity.FilePath = relativePath;

                await _StudentTablesAttachmentService.AddAsync(entity);
            }


            TempData["Success"] = "تمت إضافة الملف بنجاح";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _StudentTablesAttachmentService.GetByIdAsync(
                id,
                x => x.EducationalLevel,
                x => x.EducationalLevel.EducationalFacilities,
                x => x.Departmentsandbranches,
                x => x.TableType,
                x => x.Terms,
                x => x.Specialization
            );

            if (entity == null)
                return NotFound();

            var model = _mapper.Map<StudentTablesAttachmentVM>(entity);


            // IDs
            var facilityId = entity.EducationalLevel?.EducationalFacilitiesId;
            var levelId = entity.EducationalLevelId;
            var departmentId = entity.DepartmentsandbranchesId;

            model.EducationalFacilitiesId = facilityId ?? 0;
            await RefillViewBags(model);

     

            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentTablesAttachmentVM model)
        {
            if (!ModelState.IsValid)
            {


                TempData["Error"] = "تأكد من صحة البيانات المدخلة";
               
                await RefillViewBags(model);

                return View(model);
            }

            var entity = await _StudentTablesAttachmentService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();


            if (model.UploadedFile == null && string.IsNullOrEmpty(entity.FilePath))
            {
                ModelState.AddModelError("UploadedFile", "يجب تحميل ملف.");
                await RefillViewBags(model);

                return View(model);
            }

          
            entity.TableTypeId = model.TableTypeId;
            entity.EducationalLevelId = model.EducationalLevelId;
            entity.DepartmentsandbranchesId = model.DepartmentsandbranchesId;
            entity.TermsId = model.TermsId;
            entity.SpecializationId = model.SpecializationId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

            //  File upload
            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                string[] allowedDocs = { ".pdf", ".docx", ".xlsx" };

                var uploadedPath = await _fileStorageService.UploadFileAsync(
                    model.UploadedFile,
                    "StudentTablesAttachmentFile",
                    allowedDocs
                );

                if (uploadedPath == null)
                {
                    //ModelState.AddModelError(
                    //    "UploadedFile",
                    //    "نوع الملف غير مسموح به. يسمح فقط بـ PDF أو Word أو Excel."
                    //);

                    TempData["Error"] = "نوع الملف غير مسموح به";

                    await RefillViewBags(model);
                    return View(model);
                }

                // delete old file only AFTER successful upload
                if (!string.IsNullOrEmpty(entity.FilePath))
                {
                    await _fileStorageService.DeleteFileAsync(entity.FilePath);
                }

                entity.FilePath = uploadedPath;
            }


            await _StudentTablesAttachmentService.UpdateAsync(entity);

            TempData["Success"] = "تم تعديل الملف بنجاح";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var StudentTablesAttachment = await _StudentTablesAttachmentService.GetByIdAsync(id);
            if (StudentTablesAttachment == null) return NotFound();

            await _StudentTablesAttachmentService.DeleteAsync(id);

            TempData["Success"] = "تم حذف الملف بنجاح ";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<JsonResult> GetLevelsByFacilityId(int facilityId)
        {
            var levels = await _EducationalLevelService.GetAllAsync();
            var filteredLevels = levels
                .Where(x => x.EducationalFacilitiesId == facilityId)
                .Select(x => new { x.Id, x.NameAr })
                .ToList();
                

            return Json(filteredLevels);
        }
     
        [HttpGet]
        public async Task<JsonResult> GetDepartmentsByFacilityId(int facilityId)
        {
            var departments = await _departmentsandbranchService.GetAllAsync();

            var data = departments
                .Where(x => x.EducationalFacilitiesId == facilityId)
                .Select(x => new { x.Id, x.NameAr })
                .ToList();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetSpecializationsByDepartmentId(int departmentId)
        {
            var specializations = await _specializationService.GetAllAsync();

            var data = specializations
                .Where(x => x.DepartmentsandbranchesId == departmentId)
                .Select(x => new { x.Id, x.NameAr })
                .ToList();

            return Json(data);
        }



        private async Task RefillViewBags(StudentTablesAttachmentVM model)
        {
         
            ViewBag.EducationalFacilityList = new SelectList( await _educationalFacilityService.GetDropdownListAsync(),"Id","NameAr",model.EducationalFacilitiesId);

            ViewBag.TableTypeList = new SelectList(await _tableTypeService.GetDropdownListAsync(),"Id", "NameAr",model.TableTypeId);

            ViewBag.TermList = new SelectList(await _termService.GetDropdownListAsync(),"Id","NameAr",model.TermsId);

        
            ViewBag.EducationalLevelList = new SelectList((await _EducationalLevelService.GetAllAsync()).Where(x => x.EducationalFacilitiesId == model.EducationalFacilitiesId), "Id","NameAr",model.EducationalLevelId);

            ViewBag.departmentsandbranchList = new SelectList((await _departmentsandbranchService.GetAllAsync()).Where(x => x.EducationalFacilitiesId == model.EducationalFacilitiesId), "Id","NameAr", model.DepartmentsandbranchesId);

            ViewBag.SpecializationList = new SelectList((await _specializationService.GetAllAsync()).Where(x => x.DepartmentsandbranchesId == model.DepartmentsandbranchesId),"Id", "NameAr",model.SpecializationId);
        }



    }
}
