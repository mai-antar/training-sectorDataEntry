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
  

    public class ProjectsController : Controller
    {
        private readonly IGenericService<Project> _projectService;
        private readonly IGenericService<ProjectImage> _projectImagesService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IGenericService<EducationalFacility> _educationalFacilityService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        public ProjectsController(IGenericService<Project> projectService, IGenericService<ProjectImage> projectImagesService,
            IGenericService<EducationalFacility> educationalFacilityService, IMapper mapper, ILoggerRepository logger, IFileStorageService fileStorageService)
        {
            _projectService = projectService;
            _projectImagesService = projectImagesService;
            _educationalFacilityService = educationalFacilityService;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _logger = logger;
        } 
        public async Task<IActionResult> Index()
        {
            var ProjectsList = await _projectService.GetAllAsync();
            var projectImagesList = await _projectImagesService.GetAllAsync();

            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            foreach (var item in ProjectsList)
            {
                if (projectImagesList.Where(a => a.ProjectId == item.Id).ToList().Count > 0)
                {

                    item.ProjectImages = projectImagesList.Where(a => a.ProjectId == item.Id).ToList();
                }
            }
            var viewModelList = _mapper.Map<List<ProjectVM>>(ProjectsList);

            return View(viewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var existingProjects = await _projectService.GetAllAsync();
            var existingProjectVM = _mapper.Map<List<ProjectVM>>(existingProjects);

            var projectImagesList = await _projectImagesService.GetAllAsync();
            foreach (var item in existingProjectVM)
            {
                if (projectImagesList.Where(a => a.ProjectId == item.Id).ToList().Count > 0)
                {

                    item.ProjectImages = projectImagesList.Where(a => a.ProjectId == item.Id).ToList();
                }
            }

            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            ViewBag.existingProjects = existingProjectVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

                var existingProjects = await _projectService.GetAllAsync();
                var existingProjectVM = _mapper.Map<List<ProjectVM>>(existingProjects);

                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                ViewBag.existingProjects = existingProjectVM;

                return View(model);
            }


            var entity = _mapper.Map<Project>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

            await _projectService.AddAsync(entity);
      
            if (model.UploadedImages != null && model.UploadedImages.Any())
            {


                foreach (var file in model.UploadedImages)
                {
                    var relativePath = await _fileStorageService.UploadImageAsync(file, "ProjectImage");

                    if (relativePath != null)
                    {
                        await _projectImagesService.AddAsync(new ProjectImage
                        {
                            ProjectId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });

                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            //var Project = await _projectService.GetByIdAsync(id);
            var Project = await _projectService.GetByIdAsync(id, n => ((Project)n).ProjectImages);
            if (Project == null) return NotFound();

            var model = _mapper.Map<ProjectVM>(Project);
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
            ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectVM model)
        {
            if (!ModelState.IsValid)
            {
                var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();
                ViewBag.educationalFacilityList = new SelectList(educationalFacility, "Id", "NameAr");
                return View(model);
            }


            var entity = await _projectService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.Date = model.Date;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.EducationalFacilitiesId = model.EducationalFacilitiesId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);
            //var entity = _mapper.Map<Project>(model);


            await _projectService.UpdateAsync(entity);

            if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
            {
                foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
                {
                    var image = await _projectImagesService.GetByIdAsync(id);
                    if (image != null)
                    {
                        image.IsDeleted = true;
                        await _projectImagesService.UpdateAsync(image);
                    }
                }
            }

            if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
            {
                foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
                {
                    var image = await _projectImagesService.GetByIdAsync(id);
                    if (image != null)
                    {
                        await _fileStorageService.DeleteFileAsync(image.ImagePath);
                        await _projectImagesService.DeleteAsync(id);
                      
                    }
                }
            }

            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
                foreach (var image in model.UploadedImages)
                {

                    var relativePath = await _fileStorageService.UploadImageAsync(image, "ProjectImage");

                    if (relativePath != null)
                    {
                        await _projectImagesService.AddAsync(new ProjectImage
                        {
                            ProjectId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });

                    }
                }
            }



            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> AddImages(int id)
        {
         
            var Project = await _projectService.GetByIdAsync(id);
            if (Project == null)
                return NotFound();

            var vm = new ProjectImageVM
            {
                ProjectsId = id,
                TitleAr = Project.TitleAr
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(ProjectImageVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var Project = await _projectService.GetByIdAsync(model.ProjectsId);
            if (Project == null)
                return NotFound();

            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/ProjectImage");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var image in model.UploadedImages)
            {
                if (image.Length > 0 && image.ContentType.StartsWith("image/"))
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var ProjectImage = new ProjectImage
                    {
                        ProjectId = model.Id,
                        ImagePath = "/uploads/ProjectImage/" + fileName,
                        IsActive = true,
                        IsDeleted = false,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                    };

                    await _projectImagesService.AddAsync(ProjectImage);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _projectImagesService.GetByIdAsync(id);
            if (image == null) return NotFound();

            await _projectImagesService.DeleteAsync(id);
            return RedirectToAction("Edit", new { id = image.ProjectId });
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Project = await _projectService.GetByIdAsync(id);
            if (Project == null) return NotFound();

            await _projectService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectByFacility(int facilityId)
        {
            var educationalFacility = await _educationalFacilityService.GetDropdownListAsync();

            var projects = await _projectService.GetAllAsync(false, x => x.EducationalFacilities,
                  x => x.ProjectImages);

            projects = projects.Where(a => a.EducationalFacilitiesId == facilityId).ToList();

            var vmList = _mapper.Map<List<ProjectVM>>(projects);


            return PartialView("_ProjectsPartial", vmList);
        }


    }
}
