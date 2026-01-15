using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.Services;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class SpecializationImageController : Controller
    {
        private readonly IGenericService<SpecializationImage> _imageService;
        private readonly IGenericService<Specialization> _specializationService;
        private readonly IGenericService<Departmentsandbranch> _departmentsandbranchService;
       // private readonly IGenericService<EducationalFacility> _EducationalFacility;
       
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        public SpecializationImageController(
         IGenericService<SpecializationImage> imageService,
         IGenericService<Specialization> specializationService, IMapper mapper,
          IFileStorageService fileStorageService, IGenericService<Departmentsandbranch> departmentsandbranchService)
        {
            _imageService = imageService;
            _specializationService = specializationService;
            _departmentsandbranchService = departmentsandbranchService;
            //_EducationalFacility = EducationalFacility;
            //_DepartmentType = DepartmentType;
            _mapper = mapper;
            _fileStorageService = fileStorageService;

        }
           
        public async Task<IActionResult> Index(int specializationId)
        {

         

            //var specialization = await _specializationService.GetByIdAsync(specializationId);
            var specialization = await _specializationService.GetByIdAsync(specializationId, x => x.Departmentsandbranches);
            if (specialization == null) return NotFound();

            var department = await _departmentsandbranchService.GetByIdAsync(specialization.DepartmentsandbranchesId, x => x.EducationalFacilities, x => x.DepatmentType);

           
            ViewBag.SpecializationId = specializationId;
            ViewBag.specializationName = specialization.NameAr;

            ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;

            ViewBag.departmentName = department.DepatmentType.NameAr;

            var images = await _imageService.GetAllAsync();
            var specializationImages = images
                .Where(x => x.SpecializationId == specializationId)
                .ToList();

            var viewModelList = _mapper.Map<List<SpecializationImageVM>>(specializationImages);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create(int specializationId)
        {
            var specialization = await _specializationService.GetByIdAsync(specializationId, x => x.Departmentsandbranches);
            if (specialization == null) return NotFound();

            var department = await _departmentsandbranchService.GetByIdAsync(specialization.DepartmentsandbranchesId, x => x.EducationalFacilities, x => x.DepatmentType);

            ViewBag.SpecializationId = specializationId;
            ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
            ViewBag.specializationName =specialization.NameAr;
            ViewBag.departmentName = department.NameAr;
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecializationImageVM model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

            if (model.UploadedImage == null || model.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
            }
            else
            {
                var extension = Path.GetExtension(model.UploadedImage.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "UploadedImage",
                        "صيغة الصورة غير مدعومة. الصيغ المسموحة: jpg, jpeg, png, webp"
                    );
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.SpecializationId = model.SpecializationId;
                ViewBag.specializationName = model.specializationName;
              
                ViewBag.educationalFacilitiesName = model.educationalFacilitiesName;
                ViewBag.departmentName = model.departmentName;
                return View(model);
            }

            model.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
            model.IsActive = true;

            var entity = _mapper.Map<SpecializationImage>(model);



            if (model.UploadedImage != null)
            {

                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "SpecializationImage");
                if (relativePath == null)

                 entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                entity.ImagePath = relativePath;

                await _imageService.AddAsync(entity);
            }

            TempData["Success"] = "تمت الاضافة بنجاح";

            return RedirectToAction("Index", new { SpecializationId = model.SpecializationId,
                });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _imageService.GetByIdAsync(
                id,
                x => x.Specialization
            );

            if (entity == null)
                return NotFound();

            //var department = await _specializationService.GetByIdAsync(
            //    entity.SpecializationId,
            //    x => x.Departmentsandbranches,
            //    x => x.DepatmentType
            //);


            var specialization = await _specializationService.GetByIdAsync(entity.SpecializationId, x => x.Departmentsandbranches);
            if (specialization == null) return NotFound();

            var department = await _departmentsandbranchService.GetByIdAsync(specialization.DepartmentsandbranchesId, x => x.EducationalFacilities, x => x.DepatmentType);
            if (department == null)
                return NotFound();



            var vm = _mapper.Map<SpecializationImageVM>(entity);

            ViewBag.specializationName = specialization.NameAr;
            ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
            ViewBag.depatmentTypeName = department.DepatmentType.NameAr;
            ViewBag.DepartmentId = department.Id;
            ViewBag.DepartmentName = department.NameAr;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecializationImageVM model)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

            var entity = await _imageService.GetByIdAsync(model.Id);
            if (entity == null)
                return NotFound();

            if (model.UploadedImage == null && string.IsNullOrEmpty(entity.ImagePath))
            {
                ModelState.AddModelError("UploadedImage", "يجب تحميل صورة.");
                model.ImagePath = entity.ImagePath;

                return View(model);
            }
            // لو المستخدم رفع صورة جديدة
            if (model.UploadedImage != null && model.UploadedImage.Length > 0)
            {
                var extension = Path.GetExtension(model.UploadedImage.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "UploadedImage",
                        "صيغة الصورة غير مدعومة. الصيغ المسموحة: jpg, jpeg, png, webp"
                    );
                }
            }
            if (!ModelState.IsValid)
            {
                var specialization = await _specializationService.GetByIdAsync(
                    model.SpecializationId,
                    x => x.Departmentsandbranches
                   
                );
                var department = await _departmentsandbranchService.GetByIdAsync(specialization.DepartmentsandbranchesId, x => x.EducationalFacilities, x => x.DepatmentType);


                ViewBag.DepartmentId = specialization.DepartmentsandbranchesId;
                ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
                ViewBag.DepartmentName = department.NameAr;
                ViewBag.DepartmentName = department.NameAr;

                model.ImagePath = entity.ImagePath;

                return View(model);
            }

            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);


            if (model.UploadedImage != null)
            {
                var relativePath = await _fileStorageService.UploadImageAsync(
                    model.UploadedImage,
                    "SpecializationImage"
                );

                if (relativePath != null)
                    entity.ImagePath = relativePath;
            }

            await _imageService.UpdateAsync(entity);


            TempData["Success"] = "تم التعديل بنجاح";


            return RedirectToAction("Index", new
            {
                specializationId = model.SpecializationId,

            });
        }
        public async Task<IActionResult> Delete(int id)
        {
            var image = await _imageService.GetByIdAsync(id);
            if (image == null)
                return NotFound();

            await _imageService.DeleteAsync(id);


            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction("Index", new
            {
                specializationId = image.SpecializationId
            });
        }
    }
}
