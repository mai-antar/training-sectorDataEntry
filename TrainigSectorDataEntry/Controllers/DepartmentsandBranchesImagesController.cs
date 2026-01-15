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
  

    public class DepartmentsandBranchesImagesController : Controller
    {
        private readonly IGenericService<DepartmentsandBranchesImage> _imageService;
        private readonly IGenericService<Departmentsandbranch> _departmentService;
        private readonly IGenericService<EducationalFacility> _EducationalFacility;
        private readonly IGenericService<DepartmentType> _DepartmentType;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public DepartmentsandBranchesImagesController(
            IGenericService<DepartmentsandBranchesImage> imageService,
            IGenericService<Departmentsandbranch> departmentService, IMapper mapper, IGenericService<EducationalFacility> EducationalFacility,
            IGenericService<DepartmentType> DepartmentType, IFileStorageService fileStorageService)
        {
            _imageService = imageService;
            _departmentService = departmentService;
            _EducationalFacility = EducationalFacility;
            _DepartmentType = DepartmentType;
            _mapper = mapper;
            _fileStorageService = fileStorageService;

        }

       
        public async Task<IActionResult> Index(int departmentId)
        {
            var department = await _departmentService.GetByIdAsync( departmentId,x => x.EducationalFacilities,x => x.DepatmentType);
            if (department == null) return NotFound();

            ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
            ViewBag.depatmentTypeName = department.DepatmentType.NameAr;

            ViewBag.DepartmentName = department.NameAr;
            ViewBag.DepartmentId = departmentId;

            var images = await _imageService.GetAllAsync();
            var departmentImages = images
                .Where(x => x.DepartmentsandbranchesId == departmentId)
                .ToList();

            var viewModelList = _mapper.Map<List<DepartmentsandBranchesImageVM>>(departmentImages);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create(int departmentId)
        {
            var department = await _departmentService.GetByIdAsync(departmentId, x => x.EducationalFacilities, x => x.DepatmentType);
            if (department == null) return NotFound();

            ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
            ViewBag.depatmentTypeName = department.DepatmentType.NameAr;

            ViewBag.depatmentName = department.NameAr;
            ViewBag.DepartmentId = departmentId;

     
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentsandBranchesImageVM model) 
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
                ViewBag.DepartmentId = model.DepartmentsandbranchesId;
                ViewBag.depatmentTypeName = model.depatmentTypeName;
                ViewBag.educationalFacilitiesName = model.educationalFacilitiesName;
                return View(model);
            }

            model.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
            model.IsActive = true;

            var entity = _mapper.Map<DepartmentsandBranchesImage>(model);

           

            if (model.UploadedImage != null)
            {
                
                var relativePath = await _fileStorageService.UploadImageAsync(model.UploadedImage, "DepartmentsandBranchesImage");
                if(relativePath==null)
               
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);
                entity.ImagePath = relativePath;

                await _imageService.AddAsync(entity);
            }
            TempData["Success"] = "تمت الاضافة بنجاح";
            return RedirectToAction("Index", new { departmentId = model.DepartmentsandbranchesId  });
        }
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _imageService.GetByIdAsync(
                id,
                x => x.Departmentsandbranches
            );

            if (entity == null)
                return NotFound();

            var department = await _departmentService.GetByIdAsync(
                entity.DepartmentsandbranchesId,
                x => x.EducationalFacilities,
                x => x.DepatmentType
            );

            if (department == null)
                return NotFound();

            var vm = _mapper.Map<DepartmentsandBranchesImageVM>(entity);

            ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
            ViewBag.depatmentTypeName = department.DepatmentType.NameAr;
            ViewBag.DepartmentId = department.Id;
            ViewBag.DepartmentName = department.NameAr;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentsandBranchesImageVM model)
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
                var department = await _departmentService.GetByIdAsync(
                    model.DepartmentsandbranchesId,
                    x => x.EducationalFacilities,
                    x => x.DepatmentType
                );

                ViewBag.DepartmentId = model.DepartmentsandbranchesId;
                ViewBag.educationalFacilitiesName = department.EducationalFacilities.NameAr;
                ViewBag.depatmentTypeName = department.DepatmentType.NameAr;

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
                    "DepartmentsandBranchesImage"
                );

                if (relativePath != null)
                    entity.ImagePath = relativePath;
            }

            await _imageService.UpdateAsync(entity);

            TempData["Success"] = "تم التعديل بنجاح";
            return RedirectToAction("Index", new
            {
                departmentId = model.DepartmentsandbranchesId,
              
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
                departmentId = image.DepartmentsandbranchesId
            });
        }


    }

}
