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
        private readonly IGenericService<EducationalFacility> _EducationalFacilitiesService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public ContactUsController(IGenericService<ContactU> ContactU,
            IGenericService<EducationalFacility> EducationalFacilitiesService, IMapper mapper, ILoggerRepository logger)
        {
            _ContactUService = ContactU;
            _EducationalFacilitiesService = EducationalFacilitiesService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var ContactUList = await _ContactUService.GetAllAsync();
            var EducationalFacilities = await _EducationalFacilitiesService.GetDropdownListAsync();

            ViewBag.EducationalFacilitiesList = new SelectList(EducationalFacilities, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<ContactUVM>>(ContactUList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var EducationalFacilities = await _EducationalFacilitiesService.GetDropdownListAsync();

            var existingContactU = await _ContactUService.GetAllAsync();
            var existingContactUVM = _mapper.Map<List<ContactUVM>>(existingContactU);



            ViewBag.EducationalFacilitiesList = new SelectList(EducationalFacilities, "Id", "NameAr");

            if (TempData["ContactUS_EducationalFacilitiesId"] != null)
            {
                ViewBag.EducationalFacilitiesList = new SelectList(EducationalFacilities, "Id", "NameAr", TempData["ContactUS_EducationalFacilitiesId"]);

            }
            ViewBag.existingContactU = existingContactUVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactUVM model)
        {


            if (!ModelState.IsValid)
            {
                var EducationalFacilities = await _EducationalFacilitiesService.GetDropdownListAsync();
                var existingContactU = await _ContactUService.GetAllAsync();
                var existingContactUVM = _mapper.Map<List<ContactUVM>>(existingContactU);

                ViewBag.EducationalFacilitiesList = new SelectList(EducationalFacilities, "Id", "NameAr");
                ViewBag.existingContactU = existingContactUVM;

                return View(model);
            }
            // Map and save the entity
            var entity = _mapper.Map<ContactU>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);


            await _ContactUService.AddAsync(entity);

            TempData["Success"] = "تمت الاضافة بنجاح";
            TempData["ContactUS_EducationalFacilitiesId"] = model.EducationalFacilitiesId;

            return RedirectToAction(nameof(Create));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var ContactU = await _ContactUService.GetByIdAsync(id);
            if (ContactU == null) return NotFound();

            var model = _mapper.Map<ContactUVM>(ContactU);
            var EducationalFacilities = await _EducationalFacilitiesService.GetDropdownListAsync();
            ViewBag.EducationalFacilitiesList = new SelectList(EducationalFacilities, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactUVM model)
        {
            if (!ModelState.IsValid)
            {
                var EducationalFacilities = await _EducationalFacilitiesService.GetDropdownListAsync();
                ViewBag.EducationalFacilitiesList = new SelectList(EducationalFacilities, "Id", "NameAr");
                return View(model);
            }

            var entity = await _ContactUService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.Address = model.Address;
            entity.Telephone = model.Telephone;
            entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.Fax = model.Fax;
            entity.Email = model.Email;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);



            await _ContactUService.UpdateAsync(entity);


            TempData["Success"] = "تم التعديل بنجاح";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ContactU = await _ContactUService.GetByIdAsync(id);
            if (ContactU == null) return NotFound();

            await _ContactUService.DeleteAsync(id);
            TempData["Success"] = "تم الحذف بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
