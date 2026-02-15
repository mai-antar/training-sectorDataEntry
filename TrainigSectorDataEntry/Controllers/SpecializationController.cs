using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class SpecializationController : Controller
    {
        private readonly IGenericService<Specialization> _specializationService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IGenericService<Departmentsandbranch> _departmentsandbranch;

        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public SpecializationController(IGenericService<Specialization> specialization,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IGenericService<Departmentsandbranch> departmentsandbranch)
        {
            _specializationService = specialization;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
            _departmentsandbranch = departmentsandbranch;
        }
        public async Task<IActionResult> Index()
        {
            var specializations = await _specializationService.GetAllAsync(false, x => x.Departmentsandbranches, x => x.Departmentsandbranches.EducationalFacilities);

            var viewModelList = specializations.Select(x => new SpecializationVM
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                DepartmentsandbranchesId = x.DepartmentsandbranchesId,
                DepartmentName = x.Departmentsandbranches?.NameAr,
                EducationalFacilityName = x.Departmentsandbranches?.EducationalFacilities?.NameAr,
                IsActive = x.IsActive,
                UserCreationDate = x.UserCreationDate
            }).ToList();

            return View(viewModelList);
      
        }

        public async Task<IActionResult> Create()
        {

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var departmentsandbranch = await _departmentsandbranch.GetDropdownListAsync();


            ViewBag.departmentsandbranchList = new SelectList(departmentsandbranch, "Id", "NameAr");
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var existingSpecialization = await _specializationService.GetAllAsync(false,x => x.Departmentsandbranches,x => x.Departmentsandbranches.EducationalFacilities);
            var existingSpecializationVM = _mapper.Map<List<SpecializationVM>>(existingSpecialization);
            //var existingSpecializationVM = existingSpecialization.Select(x => new SpecializationVM
            //{
            //    Id = x.Id,
            //    NameAr = x.NameAr,
            //    NameEn = x.NameEn,
            //    DepartmentsandbranchesId = x.DepartmentsandbranchesId,
            //    DepartmentName = x.Departmentsandbranches?.NameAr,
            //    EducationalFacilityName =x.Departmentsandbranches?.EducationalFacilities?.NameAr,
            //    EducationalFacilitiesId = x.Departmentsandbranches.EducationalFacilitiesId,
            //    IsActive = x.IsActive,
            //    UserCreationDate = x.UserCreationDate
            //}).ToList();

            if (TempData["Spec_EducationalFacilitiesId"] != null && TempData["Spec_DepartmentsandbranchesId"] != null)
            {
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr", TempData["Spec_EducationalFacilitiesId"]);
                ViewBag.departmentsandbranchList = new SelectList(departmentsandbranch, "Id", "NameAr", TempData["Spec_DepartmentsandbranchesId"]);
             

            }
            ViewBag.existingSpecialization = existingSpecializationVM;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecializationVM model)
        {
           

            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingSpecialization = await _specializationService.GetAllAsync();

                var departmentsandbranch = await _departmentsandbranch.GetDropdownListAsync();

                var existingSpecializationVM = _mapper.Map<List<SpecializationVM>>(existingSpecialization);


                ViewBag.departmentsandbranchList = new SelectList(departmentsandbranch, "Id", "NameAr");
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingSpecialization = existingSpecializationVM;

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<Specialization>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
       

            await _specializationService.AddAsync(entity);


            TempData["Success"] = "تمت الاضافة بنجاح";
            TempData["Spec_EducationalFacilitiesId"] = model.EducationalFacilitiesId;
            TempData["Spec_DepartmentsandbranchesId"] = model.DepartmentsandbranchesId;


          

            return RedirectToAction(nameof(Create));
        }



        public async Task<IActionResult> Edit(int id)
        {
            var Specialization = await _specializationService.GetByIdAsync(id);
            if (Specialization == null) return NotFound();

           
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var departmentsandbranch = await _departmentsandbranch.GetByIdAsync(Specialization.DepartmentsandbranchesId);
            if (departmentsandbranch == null) return NotFound();
           

            var model = _mapper.Map<SpecializationVM>(Specialization);
            model.EducationalFacilitiesId = departmentsandbranch.EducationalFacilitiesId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecializationVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            var entity = await _specializationService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.NameAr = model.NameAr;
            entity.NameEn = model.NameEn;
           // entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _specializationService.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Specialization = await _specializationService.GetByIdAsync(id);
            if (Specialization == null) return NotFound();

            await _specializationService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetspecializationByFacilityId(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var specializations = await _specializationService.GetAllAsync(false, x => x.Departmentsandbranches, x=>x.Departmentsandbranches.EducationalFacilities);

            specializations = specializations.Where(a => a.Departmentsandbranches.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<SpecializationVM>>(specializations);


            return PartialView("_SpecializationPartial", vmList);
        }

    }
}
