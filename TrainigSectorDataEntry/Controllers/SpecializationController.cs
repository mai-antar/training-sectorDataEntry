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



            //var SpecializationList = await _specializationService.GetAllAsync();
            //var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            //ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            //var departmentsandbranch = await _departmentsandbranch.GetDropdownListAsync();

            //ViewBag.departmentsandbranchList = new SelectList(departmentsandbranch, "Id", "NameAr");

            //var viewModelList = _mapper.Map<List<SpecializationVM>>(SpecializationList);

            //return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var departmentsandbranch = await _departmentsandbranch.GetDropdownListAsync();


            //var existingSpecialization = await _specializationService.GetAllAsync();
            //var existingSpecializationVM = _mapper.Map<List<SpecializationVM>>(existingSpecialization);


            ViewBag.departmentsandbranchList = new SelectList(departmentsandbranch, "Id", "NameAr");
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var existingSpecialization = await _specializationService.GetAllAsync(false,x => x.Departmentsandbranches,x => x.Departmentsandbranches.EducationalFacilities);

            var existingSpecializationVM = existingSpecialization.Select(x => new SpecializationVM
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                DepartmentName = x.Departmentsandbranches?.NameAr,
                EducationalFacilityName =x.Departmentsandbranches?.EducationalFacilities?.NameAr,
                IsActive = x.IsActive,
                UserCreationDate = x.UserCreationDate
            }).ToList();

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

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var Specialization = await _specializationService.GetByIdAsync(id);
            if (Specialization == null) return NotFound();

            var model = _mapper.Map<SpecializationVM>(Specialization);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

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


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Specialization = await _specializationService.GetByIdAsync(id);
            if (Specialization == null) return NotFound();

            await _specializationService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<JsonResult> GetDepartmentsByFacilityId(int facilityId)
        {
            var departments = await _departmentsandbranch.GetAllAsync(
                false,
                x => x.EducationalFacilities
            );

            var result = departments
                .Where(x => x.EducationalFacilitiesId == facilityId)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.NameAr
                })
                .ToList();

            return Json(result);
        }

    }
}
