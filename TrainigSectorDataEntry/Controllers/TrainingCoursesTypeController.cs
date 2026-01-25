using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class TrainingCoursesTypeController : Controller
    {
        private readonly IGenericService<TrainingCoursesType> _TrainingCoursesTypeService;
        private readonly IGenericService<TrainingSector> _TrainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public TrainingCoursesTypeController(IGenericService<TrainingCoursesType> TrainingCoursesType,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger)
        {
            _TrainingCoursesTypeService = TrainingCoursesType;
            _TrainingSectorService = TrainingSectorService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var TrainingCoursesTypeList = await _TrainingCoursesTypeService.GetAllAsync();
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<TrainingCoursesTypeVM>>(TrainingCoursesTypeList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            var existingTrainingCoursesType = await _TrainingCoursesTypeService.GetAllAsync();
            var existingTrainingCoursesTypeVM = _mapper.Map<List<TrainingCoursesTypeVM>>(existingTrainingCoursesType);

            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");


            if (TempData["TrainingCoursesType_TrainigSectorId"] != null)
            {
                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr", TempData["TrainingCoursesType_TrainigSectorId"]);
            }

            ViewBag.existingTrainingCoursesType = existingTrainingCoursesTypeVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingCoursesTypeVM model)
        {


            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                var existingTrainingCoursesType = await _TrainingCoursesTypeService.GetAllAsync();
                var existingTrainingCoursesTypeVM = _mapper.Map<List<TrainingCoursesTypeVM>>(existingTrainingCoursesType);

                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                ViewBag.existingTrainingCoursesType = existingTrainingCoursesTypeVM;

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<TrainingCoursesType>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);


            await _TrainingCoursesTypeService.AddAsync(entity);

            TempData["Success"] = "تمت الاضافة بنجاح";
            TempData["TrainingCoursesType_TrainigSectorId"] = model.TrainingSectorId;
            return RedirectToAction(nameof(Create));
            //return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var TrainingCoursesType = await _TrainingCoursesTypeService.GetByIdAsync(id);
            if (TrainingCoursesType == null) return NotFound();

            var model = _mapper.Map<TrainingCoursesTypeVM>(TrainingCoursesType);
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TrainingCoursesTypeVM model)
        {
            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                return View(model);
            }

            var entity = await _TrainingCoursesTypeService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.NameAr = model.NameAr;
            entity.NameEn = model.NameEn;
            entity.TrainingSectorId = model.TrainingSectorId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _TrainingCoursesTypeService.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, string returnTo)
        {
            var TrainingCoursesType = await _TrainingCoursesTypeService.GetByIdAsync(id);
            if (TrainingCoursesType == null) return NotFound();

            await _TrainingCoursesTypeService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return returnTo == "Create" ? RedirectToAction(nameof(Create)) : RedirectToAction(nameof(Index));
           // return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetTrainingCoursesTypeTrainingSectorId(int trainingSectorId)
        {

            var sectors = await _TrainingSectorService.GetDropdownListAsync();
            var TrainingSector = await _TrainingCoursesTypeService.GetAllAsync();
            TrainingSector = TrainingSector.Where(a => a.TrainingSectorId == trainingSectorId).ToList();

            var vmList = _mapper.Map<List<TrainingCoursesTypeVM>>(TrainingSector);


            return PartialView("_TrainingCoursesTypePartial", vmList);
        }
    }
}
