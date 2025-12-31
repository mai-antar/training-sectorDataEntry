using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class DepartmentsandbranchesController : Controller
    {

        private readonly IGenericService<Departmentsandbranch> _DepartmentsandbranchService;
  

        private readonly IGenericService<Specialization> _specializationService;
        private readonly IGenericService<EducationalFacility> _EducationalFacility;
        private readonly IGenericService<DepartmentType> _DepartmentType;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public DepartmentsandbranchesController(IGenericService<Departmentsandbranch> Departmentsandbranch,
            IGenericService<Specialization> specializationService, IGenericService<EducationalFacility> EducationalFacility,
            IGenericService<DepartmentType> DepartmentType,IMapper mapper, ILoggerRepository logger)
        {
            _DepartmentsandbranchService = Departmentsandbranch;
       

            _specializationService = specializationService;
            _EducationalFacility=EducationalFacility;
            _DepartmentType= DepartmentType;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var DepartmentsandbranchList = await _DepartmentsandbranchService.GetAllAsync();

      

            var EducationalFacility = await _EducationalFacility.GetDropdownListAsync();
            ViewBag.EducationalFacilityList = new SelectList(EducationalFacility, "Id", "NameAr");

            var DepartmentType = await _DepartmentType.GetDropdownListAsync();
            ViewBag.DepartmentTypeList = new SelectList(DepartmentType, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<DepartmentsandbranchVM>>(DepartmentsandbranchList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {

            var model = new DepartmentsandbranchVM();
            var EducationalFacility = await _EducationalFacility.GetDropdownListAsync();
            ViewBag.EducationalFacilityList = new SelectList(EducationalFacility, "Id", "NameAr");

            var DepartmentType = await _DepartmentType.GetDropdownListAsync();
            ViewBag.DepartmentTypeList = new SelectList(DepartmentType, "ID", "NameAr");

            var existingDepartmentsandbranch = await _DepartmentsandbranchService.GetAllAsync();
            var existingDepartmentsandbranchVM = _mapper.Map<List<DepartmentsandbranchVM>>(existingDepartmentsandbranch);
            ViewBag.existingDepartmentsandbranch = existingDepartmentsandbranchVM;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(DepartmentsandbranchVM model)
        {


            if (!ModelState.IsValid)
            {
           

                var existingDepartmentsandbranch = await _DepartmentsandbranchService.GetAllAsync();
                var existingDepartmentsandbranchVM = _mapper.Map<List<DepartmentsandbranchVM>>(existingDepartmentsandbranch);
                ViewBag.existingDepartmentsandbranch = existingDepartmentsandbranchVM;

                var EducationalFacility = await _EducationalFacility.GetDropdownListAsync();
                ViewBag.EducationalFacilityList = new SelectList(EducationalFacility, "Id", "NameAr");

                var DepartmentType = await _DepartmentType.GetDropdownListAsync();
                ViewBag.DepartmentTypeList = new SelectList(DepartmentType, "ID", "NameAr");

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<Departmentsandbranch>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);


            await _DepartmentsandbranchService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int Id)
        {
            var DepartmentType = await _DepartmentType.GetDropdownListAsync();
            ViewBag.DepartmentTypeList = new SelectList(DepartmentType, "ID", "NameAr");

            var EducationalFacility = await _EducationalFacility.GetDropdownListAsync();
            ViewBag.EducationalFacilityList = new SelectList(EducationalFacility, "Id", "NameAr");

            var Departmentsandbranch = await _DepartmentsandbranchService.GetByIdAsync(Id);
            if (Departmentsandbranch == null) return NotFound();

            var model = _mapper.Map<DepartmentsandbranchVM>(Departmentsandbranch);

            var educationalFacilityspecialization = EducationalFacility.Where(a => a.Id == Departmentsandbranch.EducationalFacilitiesId).ToList();




            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(DepartmentsandbranchVM model)
        {
            if (!ModelState.IsValid)
            {
     

                var EducationalFacility = await _EducationalFacility.GetDropdownListAsync();
                ViewBag.EducationalFacilityList = new SelectList(EducationalFacility, "Id", "NameAr");

                var DepartmentType = await _DepartmentType.GetDropdownListAsync();
                ViewBag.DepartmentTypeList = new SelectList(DepartmentType, "ID", "NameAr");

                return View(model);
            }

            var entity = await _DepartmentsandbranchService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.NameAr = model.NameAr;
            entity.NameEn = model.NameEn;
            entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.DepatmentTypeID=model.DepatmentTypeID;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _DepartmentsandbranchService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
           
            var Departmentsandbranch = await _DepartmentsandbranchService.GetByIdAsync(Id);
            if (Departmentsandbranch == null) return NotFound();

            await _DepartmentsandbranchService.DeleteAsync(Id);
        

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetDepartmentByFacility(int facilityId)
        {
            var educationalFacility = await _EducationalFacility.GetDropdownListAsync();
            var departments = await _DepartmentsandbranchService.GetAllAsync(false,x => x.EducationalFacilities,
                  x => x.DepatmentType);
      
            departments = departments.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<DepartmentsandbranchVM>>(departments);


            return PartialView("_DepartmentsandbranchesPartial", vmList);
        }


    }
}
