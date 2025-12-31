using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class StudentActiviteController : Controller
    {

        private readonly IGenericService<StudentActivite> _StudentActiviteService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public StudentActiviteController(IGenericService<StudentActivite> StudentActiviteService,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger)
        {
            _StudentActiviteService = StudentActiviteService;
            _educationalFacilityService = educationalFacilityService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var StudentActiviteList = await _StudentActiviteService.GetAllAsync();
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<StudentActiviteVM>>(StudentActiviteList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingStudentActivite = await _StudentActiviteService.GetAllAsync();
            var existingStudentActiviteVM = _mapper.Map<List<StudentActiviteVM>>(existingStudentActivite);



            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.existingStudentActivite = existingStudentActiviteVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentActiviteVM model)
        {
            // Validate that an image is uploaded
            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }

            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                var existingStudentActivite = await _StudentActiviteService.GetAllAsync();
                var existingStudentActiviteVM = _mapper.Map<List<StudentActiviteVM>>(existingStudentActivite);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingStudentActivite = existingStudentActiviteVM;

                return View(model);
            }

            // Save the image
            string? fileName = null;
            if (model.UploadedImage != null)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/StudentActiviteImage");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                if (model.UploadedImage.Length > 0 && model.UploadedImage.ContentType.StartsWith("image/"))
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UploadedImage.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.UploadedImage.CopyToAsync(stream);
                    }
                }
            }

            // Map and save the entity
            var entity = _mapper.Map<StudentActivite>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
            entity.ImagePath = "/uploads/StudentActiviteImage/" + fileName;

            await _StudentActiviteService.AddAsync(entity);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var StudentActivite = await _StudentActiviteService.GetByIdAsync(id);
            if (StudentActivite == null) return NotFound();

            var model = _mapper.Map<StudentActiviteVM>(StudentActivite);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentActiviteVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            var entity = await _StudentActiviteService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            // If no new image uploaded AND no existing image, throw validation error
            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                return View(model);
            }



            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);


            if (model.UploadedImage != null && model.UploadedImage.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entity.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/StudentActiviteImage");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.UploadedImage.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadedImage.CopyToAsync(stream);
                }

                // Update entity path
                entity.ImagePath = "/uploads/StudentActiviteImage/" + fileName;
            }

            // Ensure image path is still set
            if (string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }

            await _StudentActiviteService.UpdateAsync(entity);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var StudentActivite = await _StudentActiviteService.GetByIdAsync(id);
            if (StudentActivite == null) return NotFound();

            await _StudentActiviteService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
