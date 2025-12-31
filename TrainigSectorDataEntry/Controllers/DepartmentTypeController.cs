using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class DepartmentTypeController : Controller
    {
        private readonly IGenericService<DepartmentType> _DepartmentTypeService;
        private readonly IGenericService<Specialization> _specializationService;
        private readonly IGenericService<Departmentsandbranch> _DepartmentsandbranchService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public DepartmentTypeController(IGenericService<DepartmentType> DepartmentType,IGenericService<Departmentsandbranch> Departmentsandbranch,
            IGenericService<Specialization> specializationService, IMapper mapper, ILoggerRepository logger)
        {
            _DepartmentTypeService = DepartmentType;
            _specializationService = specializationService;
            _DepartmentsandbranchService = Departmentsandbranch;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var DepartmentTypeList = await _DepartmentTypeService.GetAllAsync();
            var specialization = await _specializationService.GetDropdownListAsync();

            ViewBag.specializationList = new SelectList(specialization, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<DepartmentTypeVM>>(DepartmentTypeList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var specialization = await _specializationService.GetDropdownListAsync();

            var existingDepartmentType = await _DepartmentTypeService.GetAllAsync();
            var existingDepartmentTypeVM = _mapper.Map<List<DepartmentTypeVM>>(existingDepartmentType);



            ViewBag.specializationList = new SelectList(specialization, "Id", "NameAr");
            ViewBag.existingDepartmentType = existingDepartmentTypeVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(DepartmentTypeVM model)
        {


                if (!ModelState.IsValid)
                {
                var specialization = await _specializationService.GetDropdownListAsync();
                var existingDepartmentType = await _DepartmentTypeService.GetAllAsync();
                var existingDepartmentTypeVM = _mapper.Map<List<DepartmentTypeVM>>(existingDepartmentType);

                ViewBag.specializationList = new SelectList(specialization, "Id", "NameAr");
                ViewBag.existingDepartmentType = existingDepartmentTypeVM;

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<DepartmentType>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);


            await _DepartmentTypeService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int Id)
        {
            var DepartmentType = await _DepartmentTypeService.GetByIdAsync(Id);
            if (DepartmentType == null) return NotFound();

            var model = _mapper.Map<DepartmentTypeVM>(DepartmentType);
            var specialization = await _specializationService.GetDropdownListAsync();
            ViewBag.specializationList = new SelectList(specialization, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(DepartmentTypeVM model)
        {
            if (!ModelState.IsValid)
            {
                var specialization = await _specializationService.GetDropdownListAsync();
                ViewBag.specializationList = new SelectList(specialization, "Id", "NameAr");
                return View(model);
            }

            var entity = await _DepartmentTypeService.GetByIdAsync(model.ID);
            if (entity == null) return NotFound();

            entity.NameAr = model.NameAr;
            entity.NameEn = model.NameEn;
            //entity.SpecializationId = model.SpecializationId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _DepartmentTypeService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var DepartmentType = await _DepartmentTypeService.GetByIdAsync(Id);
            if (DepartmentType == null) return NotFound();

            await _DepartmentTypeService.DeleteAsync(Id);

            //delete related details 
            var children = await _DepartmentsandbranchService
                .GetAllAsync();
            foreach (var detail in children.Where(c => c.DepatmentTypeID == Id))
            {

                await _DepartmentsandbranchService.DeleteAsync(detail.Id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
