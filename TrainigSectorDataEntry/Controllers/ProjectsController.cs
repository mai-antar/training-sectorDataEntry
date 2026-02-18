using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.DataContext;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.Services;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
  

    public class ProjectsController : Controller
    {
        private readonly IGenericService<Project> _projectService;
        private readonly IGenericService<ProjectImage> _projectImagesService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEntityImageService _entityImageService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly TrainingSectorDbContext _context;
        public ProjectsController(IGenericService<Project> projectService, IGenericService<ProjectImage> projectImagesService,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService, 
            IEntityImageService entityImageService, TrainingSectorDbContext context)
        {
            _projectService = projectService;
            _projectImagesService = projectImagesService;
            _educationalFacilityService = educationalFacilityService;
            _fileStorageService = fileStorageService;
            _entityImageService = entityImageService;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        } 
        public async Task<IActionResult> Index()
        {
            var ProjectsList = await _projectService.GetAllAsync();
        

            var projectImagesList = await _entityImageService.FindAsync(
                x => x.EntityImagesTableTypeId == 1 && x.IsDeleted != true);

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

            var viewModelList = _mapper.Map<List<ProjectVM>>(ProjectsList);

     
            foreach (var item in viewModelList)
            {
                if (projectImagesList.Where(a => a.EntityId == item.Id).ToList().Count > 0)
                {

                    item.Images = projectImagesList.Where(a => a.EntityId == item.Id).ToList();
                }
            }
            

            return View(viewModelList);
        }

        //public async Task<IActionResult> Create()
        //{
        //    var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
        //    ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");

        //    var existingProjects = await _projectService.GetAllAsync();
        //    var existingProjectVM = _mapper.Map<List<ProjectVM>>(existingProjects);

        //    var projectImagesList = await _projectImagesService.GetAllAsync();
        //    foreach (var item in existingProjectVM)
        //    {
        //        if (projectImagesList.Where(a => a.ProjectId == item.Id).ToList().Count > 0)
        //        {

        //            item.ProjectImages = projectImagesList.Where(a => a.ProjectId == item.Id).ToList();
        //        }
        //    }
        //    if (TempData["educationalFacility"] != null )
        //    {
        //        ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr", TempData["educationalFacility"]);
        //    }
   
        //    ViewBag.existingProjects = existingProjectVM;
        //    return View();
        //}
        public async Task<IActionResult> Create()
        {
            // Dropdown
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList =
                new SelectList(educationalFacility, "Id", "NameAr");

            // Existing projects
            var existingProjects = await _projectService.GetAllAsync();
            var existingProjectVM = _mapper.Map<List<ProjectVM>>(existingProjects);

            //  Load ONLY project images 
            var projectImages = await _entityImageService.FindAsync(
                x => x.EntityImagesTableTypeId == 1 && x.IsDeleted==false
            );

            //  Attach images to each project
            foreach (var project in existingProjectVM)
            {
                project.Images = projectImages
                    .Where(x => x.EntityId == project.Id)
                    .ToList();
            }

            // Preserve selected facility
            if (TempData["educationalFacility"] != null)
            {
                ViewBag.educationalFacilityList = new SelectList(
                    educationalFacility,
                    "Id",
                    "NameAr",
                    TempData["educationalFacility"]);
            }

            ViewBag.existingProjects = existingProjectVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = _mapper.Map<Project>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

                await _projectService.AddAsync(entity);

                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        1,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();

                TempData["Success"] = "تمت الإضافة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(ProjectsController), nameof(Create));
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ، تم إلغاء العملية.");

                return View(model);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var Project = await _projectService.GetByIdAsync(id);

            var model = _mapper.Map<ProjectVM>(Project);

            var projectImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 1 && x.EntityId == id && x.IsDeleted == false);

            model.Images = projectImages;
            //var Project = await _projectService.GetByIdAsync(id, n => ((Project)n).ProjectImages);
            //if (Project == null) return NotFound();

         
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _projectService.GetByIdAsync(model.Id);
                if (entity == null) return NotFound();

                entity.TitleAr = model.TitleAr;
                entity.TitleEn = model.TitleEn;
                entity.DescriptionAr = model.DescriptionAr;
                entity.DescriptionEn = model.DescriptionEn;
                entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
                entity.IsActive = model.IsActive;
                entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);

                await _projectService.UpdateAsync(entity);

                //  حذف صور
                if (model.DeletedImageIds != null)
                {
                    foreach (var imageId in model.DeletedImageIds
                        .Where(x => x.HasValue)
                        .Select(x => x.Value))
                    {
                        await _entityImageService.DeleteImageAsync(imageId);
                    }
                }

                //  إضافة صور
                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        1,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();

                TempData["Success"] = "تم التعديل بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex,nameof(ProjectsController),nameof(Edit));
                ModelState.AddModelError("", "حدث خطأ أثناء التعديل، تم إلغاء العملية.");

                return View(model);
            }
        }

        //public async Task<IActionResult> AddImages(int id)
        //{

        //    var Project = await _projectService.GetByIdAsync(id);
        //    if (Project == null)
        //        return NotFound();

        //    var vm = new ProjectImageVM
        //    {
        //        ProjectsId = id,
        //        TitleAr = Project.TitleAr
        //    };

        //    return View(vm);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddImages(ProjectImageVM model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var project = await _projectService.GetByIdAsync(model.ProjectsId);
        //    if (project == null)
        //        return NotFound();

        //    if (model.UploadedImages != null && model.UploadedImages.Any())
        //    {
        //        foreach (var image in model.UploadedImages)
        //        {
        //            if (image != null && image.Length > 0)
        //            {
        //                // رفع الصورة باستخدام FileStorageService
        //                var relativePath = await _fileStorageService
        //                    .UploadImageAsync(image, "ProjectImage");

        //                if (string.IsNullOrEmpty(relativePath))
        //                {
        //                    ModelState.AddModelError("UploadedImages", "حدث خطأ أثناء رفع إحدى الصور.");
        //                    return View(model);
        //                }

        //                var projectImage = new ProjectImage
        //                {
        //                    ProjectId = model.ProjectsId,
        //                    ImagePath = relativePath,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
        //                };

        //                await _projectImagesService.AddAsync(projectImage);
        //            }
        //        }
        //    }

        //    return RedirectToAction(nameof(Index));
        //}



        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {
    

            var image = await _entityImageService.GetByIdAsync(id);
            if (image == null) return NotFound();
            await _entityImageService.DeleteImageAsync(id);

            //// Delete physical file
            //await _fileStorageService.DeleteFileAsync(image.ImagePath);
            //await _entityImageService.DeleteAsync(id);
            return RedirectToAction("Edit", new { id = image.EntityId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Project = await _projectService.GetByIdAsync(id);
            if (Project == null) return NotFound();

            await _projectService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();


            var projects = await _projectService.GetAllAsync();

            projects = projects.Where(a => a.EducationalFacilitiesId == facilityId).ToList();
            var vmList = _mapper.Map<List<ProjectVM>>(projects);

            var projectImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 1 && x.IsDeleted == false);

            foreach (var vm in vmList)
            {
                vm.Images = projectImages.Where(x => x.EntityId == vm.Id).ToList();
            }


            return PartialView("_ProjectsPartial", vmList);
        }


    }
}
