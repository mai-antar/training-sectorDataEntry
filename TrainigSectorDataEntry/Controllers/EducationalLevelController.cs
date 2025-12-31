using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;


namespace TrainigSectorDataEntry.Controllers
{
    public class EducationalLevelController : Controller
    {
        private readonly IGenericService<EducationalLevel> _EducationalLevelService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public EducationalLevelController(IGenericService<EducationalLevel> EducationalLevel,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger)
        {
            _EducationalLevelService = EducationalLevel;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var EducationalLevelList = await _EducationalLevelService.GetAllAsync();
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<EducationalLevelVM>>(EducationalLevelList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingEducationalLevel = await _EducationalLevelService.GetAllAsync();
            var existingEducationalLevelVM = _mapper.Map<List<EducationalLevelVM>>(existingEducationalLevel);



            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.existingEducationalLevel = existingEducationalLevelVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EducationalLevelVM model)
        {


            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingEducationalLevel = await _EducationalLevelService.GetAllAsync();
                var existingEducationalLevelVM = _mapper.Map<List<EducationalLevelVM>>(existingEducationalLevel);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingEducationalLevel = existingEducationalLevelVM;

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<EducationalLevel>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);


            await _EducationalLevelService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var EducationalLevel = await _EducationalLevelService.GetByIdAsync(id);
            if (EducationalLevel == null) return NotFound();

            var model = _mapper.Map<EducationalLevelVM>(EducationalLevel);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EducationalLevelVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            var entity = await _EducationalLevelService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.NameAr = model.NameAr;
            entity.NameEn = model.NameEn;
            entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _EducationalLevelService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var EducationalLevel = await _EducationalLevelService.GetByIdAsync(id);
            if (EducationalLevel == null) return NotFound();

            await _EducationalLevelService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetEducationalLevelByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            var departments = await _EducationalLevelService.GetAllAsync(false, x => x.EducationalFacilities);

            departments = departments.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<EducationalLevelVM>>(departments);


            return PartialView("_EducationalLevelPartial", vmList);
        }
    }
}
