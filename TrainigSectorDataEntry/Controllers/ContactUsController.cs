using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly IGenericService<ContactU> _ContactUService;
        private readonly IGenericService<TrainingSector> _TrainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public ContactUsController(IGenericService<ContactU> ContactU,
            IGenericService<TrainingSector> TrainingSectorService, IMapper mapper, ILoggerRepository logger)
        {
            _ContactUService = ContactU;
            _TrainingSectorService = TrainingSectorService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var ContactUList = await _ContactUService.GetAllAsync();
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<ContactUVM>>(ContactUList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();

            var existingContactU = await _ContactUService.GetAllAsync();
            var existingContactUVM = _mapper.Map<List<ContactUVM>>(existingContactU);



            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            ViewBag.existingContactU = existingContactUVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactUVM model)
        {


            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                var existingContactU = await _ContactUService.GetAllAsync();
                var existingContactUVM = _mapper.Map<List<ContactUVM>>(existingContactU);

                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                ViewBag.existingContactU = existingContactUVM;

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<ContactU>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);


            await _ContactUService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var ContactU = await _ContactUService.GetByIdAsync(id);
            if (ContactU == null) return NotFound();

            var model = _mapper.Map<ContactUVM>(ContactU);
            var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactUVM model)
        {
            if (!ModelState.IsValid)
            {
                var TrainingSector = await _TrainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(TrainingSector, "Id", "NameAr");
                return View(model);
            }

            var entity = await _ContactUService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.Address = model.Address;
            entity.Telephone = model.Telephone;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.Fax = model.Fax;
            entity.Email = model.Email;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _ContactUService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ContactU = await _ContactUService.GetByIdAsync(id);
            if (ContactU == null) return NotFound();

            await _ContactUService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
