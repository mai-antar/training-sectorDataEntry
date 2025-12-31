//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using TrainigSectorDataEntry.Interface;
//using TrainigSectorDataEntry.Logging;
//using TrainigSectorDataEntry.Models;
//using TrainigSectorDataEntry.ViewModel;

//namespace TrainigSectorDataEntry.Controllers
//{
//    public class DepartmentsandBrancheDetailsController : Controller
//    {
//        private readonly IGenericService<DepartmentsandBranchesDetail> _DepartmentsandBranchesDetailService;
//        private readonly IGenericService<DepartmentsandBranchesImage> _DepartmentsandBranchesImagesService;

//        private readonly IGenericService<Departmentsandbranch> _DepartmentsandbranchService;
//        private readonly IMapper _mapper;
//        private readonly ILoggerRepository _logger;
//        public DepartmentsandBrancheDetailsController(IGenericService<DepartmentsandBranchesDetail> DepartmentsandBranchesDetailService, IGenericService<DepartmentsandBranchesImage> DepartmentsandBranchesImagesService,
//            IGenericService<Departmentsandbranch> DepartmentsandbranchService, IMapper mapper, ILoggerRepository logger)
//        {
//            _DepartmentsandBranchesDetailService = DepartmentsandBranchesDetailService;
//            _DepartmentsandBranchesImagesService = DepartmentsandBranchesImagesService;
//            _DepartmentsandbranchService = DepartmentsandbranchService;
//            _mapper = mapper;
//            _logger = logger;
//        }
//        public async Task<IActionResult> Index()
//        {
//            var DepartmentsandBranchesDetailsList = await _DepartmentsandBranchesDetailService.GetAllAsync();
//            var DepartmentsandBranchesImagesList = await _DepartmentsandBranchesImagesService.GetAllAsync();

//            var Departmentsandbranch = await _DepartmentsandbranchService.GetDropdownListAsync();

//            ViewBag.DepartmentsandbranchList = new SelectList(Departmentsandbranch, "Id", "NameAr");
//            foreach (var item in DepartmentsandBranchesDetailsList)
//            {
//                if (DepartmentsandBranchesImagesList.Where(a => a.DepartmentsandBranchesDetailsId == item.Id).ToList().Count > 0)
//                {

//                    item.DepartmentsandBranchesImages = DepartmentsandBranchesImagesList.Where(a => a.DepartmentsandBranchesDetailsId == item.Id).ToList();
//                }
//            }
//            var viewModelList = _mapper.Map<List<DepartmentsandBranchesDetailVM>>(DepartmentsandBranchesDetailsList);

//            return View(viewModelList);
//        }

//        public async Task<IActionResult> Create()
//        {
//            var Departmentsandbranch = await _DepartmentsandbranchService.GetDropdownListAsync();

//            var existingDepartmentsandBranchesDetails = await _DepartmentsandBranchesDetailService.GetAllAsync();
//            var existingDepartmentsandBranchesDetailVM = _mapper.Map<List<DepartmentsandBranchesDetailVM>>(existingDepartmentsandBranchesDetails);

//            var DepartmentsandBranchesImagesList = await _DepartmentsandBranchesImagesService.GetAllAsync();
//            foreach (var item in existingDepartmentsandBranchesDetailVM)
//            {
//                if (DepartmentsandBranchesImagesList.Where(a => a.DepartmentsandBranchesDetailsId == item.Id).ToList().Count > 0)
//                {

//                    item.DepartmentsandBranchesImages = DepartmentsandBranchesImagesList.Where(a => a.DepartmentsandBranchesDetailsId == item.Id).ToList();
//                }
//            }

//            ViewBag.DepartmentsandbranchList = new SelectList(Departmentsandbranch, "Id", "NameAr");
//            ViewBag.existingDepartmentsandBranchesDetails = existingDepartmentsandBranchesDetailVM;
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(DepartmentsandBranchesDetailVM model)
//        {
//            if (!ModelState.IsValid)
//            {
//                var Departmentsandbranch = await _DepartmentsandbranchService.GetDropdownListAsync();

//                var existingDepartmentsandBranchesDetails = await _DepartmentsandBranchesDetailService.GetAllAsync();
//                var existingDepartmentsandBranchesDetailVM = _mapper.Map<List<DepartmentsandBranchesDetailVM>>(existingDepartmentsandBranchesDetails);

//                ViewBag.DepartmentsandbranchList = new SelectList(Departmentsandbranch, "Id", "NameAr");
//                ViewBag.existingDepartmentsandBranchesDetails = existingDepartmentsandBranchesDetailVM;

//                return View(model);
//            }


//            var entity = _mapper.Map<DepartmentsandBranchesDetail>(model);
//            entity.IsDeleted = false;
//            entity.IsActive = true;
//            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

//            await _DepartmentsandBranchesDetailService.AddAsync(entity);
//            if (model.UploadedImages != null && model.UploadedImages.Any())
//            {
//                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/DepartmentsandBranchesImage");
//                if (!Directory.Exists(uploadDir))
//                    Directory.CreateDirectory(uploadDir);

//                foreach (var file in model.UploadedImages)
//                {
//                    if (file.Length > 0 && file.ContentType.StartsWith("image/"))
//                    {
//                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
//                        var filePath = Path.Combine(uploadDir, fileName);

//                        using (var stream = new FileStream(filePath, FileMode.Create))
//                        {
//                            await file.CopyToAsync(stream);
//                        }

//                        var imageEntity = new DepartmentsandBranchesImage
//                        {
//                            DepartmentsandBranchesDetailsId = entity.Id,
//                            ImagePath = "/uploads/DepartmentsandBranchesImage/" + fileName,
//                            IsActive = true,
//                            IsDeleted = false,
//                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
//                        };

//                        await _DepartmentsandBranchesImagesService.AddAsync(imageEntity);
//                    }
//                }
//            }

//            return RedirectToAction(nameof(Index));
//        }

//        public async Task<IActionResult> Edit(int id)
//        {
//            //var DepartmentsandBranchesDetail = await _DepartmentsandBranchesDetailService.GetByIdAsync(id);
//            var DepartmentsandBranchesDetail = await _DepartmentsandBranchesDetailService.GetByIdAsync(id, n => ((DepartmentsandBranchesDetail)n).DepartmentsandBranchesImages);
//            if (DepartmentsandBranchesDetail == null) return NotFound();

//            var model = _mapper.Map<DepartmentsandBranchesDetailVM>(DepartmentsandBranchesDetail);
//            var Departmentsandbranch = await _DepartmentsandbranchService.GetDropdownListAsync();
//            ViewBag.DepartmentsandbranchList = new SelectList(Departmentsandbranch, "Id", "NameAr");
//            return View(model);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(DepartmentsandBranchesDetailVM model)
//        {
//            if (!ModelState.IsValid)
//            {
//                var Departmentsandbranch = await _DepartmentsandbranchService.GetDropdownListAsync();
//                ViewBag.DepartmentsandbranchList = new SelectList(Departmentsandbranch, "Id", "NameAr");
//                return View(model);
//            }


//            var entity = await _DepartmentsandBranchesDetailService.GetByIdAsync(model.Id);
//            if (entity == null) return NotFound();

//            entity.NameAr = model.NameAr;
//            entity.NameEn = model.NameEn;
//            entity.DepartmentsandBranchesId = model.DepartmentsandBranchesId;
//            entity.IsActive = model.IsActive;
//            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);
//            //var entity = _mapper.Map<DepartmentsandBranchesDetail>(model);


//            await _DepartmentsandBranchesDetailService.UpdateAsync(entity);

//            if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
//            {
//                foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
//                {
//                    var image = await _DepartmentsandBranchesImagesService.GetByIdAsync(id);
//                    if (image != null)
//                    {
//                        image.IsDeleted = true;
//                        await _DepartmentsandBranchesImagesService.UpdateAsync(image);
//                    }
//                }
//            }



//            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/DepartmentsandBranchesImage");
//            if (!Directory.Exists(uploadDir))
//                Directory.CreateDirectory(uploadDir);

//            if (model.UploadedImages != null && model.UploadedImages.Any())
//            {
//                foreach (var image in model.UploadedImages)
//                {
//                    if (image.Length > 0 && image.ContentType.StartsWith("image/"))
//                    {
//                        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
//                        var filePath = Path.Combine(uploadDir, fileName);

//                        using (var stream = new FileStream(filePath, FileMode.Create))
//                        {
//                            await image.CopyToAsync(stream);
//                        }

//                        var DepartmentsandBranchesImage = new DepartmentsandBranchesImage
//                        {
//                            DepartmentsandBranchesDetailsId = model.Id,
//                            ImagePath = "/uploads/DepartmentsandBranchesImage/" + fileName,
//                            IsActive = true,
//                            IsDeleted = false,
//                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
//                        };

//                        await _DepartmentsandBranchesImagesService.AddAsync(DepartmentsandBranchesImage);
//                    }
//                }
//            }



//            return RedirectToAction(nameof(Index));
//        }
//        public async Task<IActionResult> AddImages(int id)
//        {
//            var DepartmentsandBranchesDetail = await _DepartmentsandBranchesDetailService.GetByIdAsync(id);
//            if (DepartmentsandBranchesDetail == null)
//                return NotFound();

//            var vm = new DepartmentsandBranchesImageVM
//            {
//                DepartmentsandBranchesDetailsId = id,
//                DepartmentsandBranchesDetailsName= DepartmentsandBranchesDetail.NameAr
//            };

//            return View(vm);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> AddImages(DepartmentsandBranchesImageVM model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            var DepartmentsandBranchesDetail = await _DepartmentsandBranchesDetailService.GetByIdAsync(model.DepartmentsandBranchesDetailsId);
//            if (DepartmentsandBranchesDetail == null)
//                return NotFound();

//            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/DepartmentsandBranchesImage");
//            if (!Directory.Exists(uploadDir))
//                Directory.CreateDirectory(uploadDir);

//            foreach (var image in model.UploadedImages)
//            {
//                if (image.Length > 0 && image.ContentType.StartsWith("image/"))
//                {
//                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
//                    var filePath = Path.Combine(uploadDir, fileName);

//                    using (var stream = new FileStream(filePath, FileMode.Create))
//                    {
//                        await image.CopyToAsync(stream);
//                    }

//                    var DepartmentsandBranchesImage = new DepartmentsandBranchesImage
//                    {
//                        DepartmentsandBranchesDetailsId = model.Id,
//                        ImagePath = "/uploads/DepartmentsandBranchesImage/" + fileName,
//                        IsActive = true,
//                        IsDeleted = false,
//                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
//                    };

//                    await _DepartmentsandBranchesImagesService.AddAsync(DepartmentsandBranchesImage);
//                }
//            }

//            return RedirectToAction(nameof(Index));
//        }
//        [HttpGet]
//        public async Task<IActionResult> DeleteImage(int id)
//        {
//            var image = await _DepartmentsandBranchesImagesService.GetByIdAsync(id);
//            if (image == null) return NotFound();

//            await _DepartmentsandBranchesImagesService.DeleteAsync(id);
//            return RedirectToAction("Edit", new { id = image.DepartmentsandBranchesDetailsId });
//        }
//        public async Task<IActionResult> Delete(int id)
//        {
//            var DepartmentsandBranchesDetail = await _DepartmentsandBranchesDetailService.GetByIdAsync(id);
//            if (DepartmentsandBranchesDetail == null) return NotFound();

//            await _DepartmentsandBranchesDetailService.DeleteAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
