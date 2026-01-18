using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;
using System.Configuration;
using System.Net.Mail;
using TrainigSectorDataEntry.Helper;
using TrainigSectorDataEntry.Services;

namespace TrainigSectorDataEntry.Controllers
{
    public class NewsController : Controller
    {
        private readonly IGenericService<News> _newsService;
        private readonly IGenericService<NewsImage> _newsImagesService;
      
        private readonly IGenericService<TrainingSector> _trainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService;
        public NewsController(IGenericService<News> newsService, IGenericService<NewsImage> newsImagesService,IGenericService<TrainingSector> trainingSectorService,
            IMapper mapper, ILoggerRepository logger, IConfiguration configuration, IFileStorageService fileStorageService)
        {
            _newsService = newsService;
            _newsImagesService = newsImagesService;
            _trainingSectorService = trainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var newsList = await _newsService.GetAllAsync(false,x=>x.TrainigSector, x => x.NewsImages);
            var vm = _mapper.Map<List<NewsVM>>(newsList);

            //var vm = _mapper.Map<NewsVM>(newsList);
        

            return View(vm);


            
        }

        public async Task<IActionResult> Create()
        {
            var sectors = await _trainingSectorService.GetDropdownListAsync();

            var existingNews = await _newsService.GetAllAsync();
            var existingNewsVM = _mapper.Map<List<NewsVM>>(existingNews);

            var newsImagesList = await _newsImagesService.GetAllAsync();
            foreach (var item in existingNewsVM)
            {
                if (newsImagesList.Where(a => a.NewsId == item.Id).ToList().Count > 0)
                {

                    item.NewsImages = newsImagesList.Where(a => a.NewsId == item.Id).ToList();
                }
            }

            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
            ViewBag.ExistingNews = existingNewsVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsVM model)
        {
            if (!ModelState.IsValid)
            {
                var sectors = await _trainingSectorService.GetDropdownListAsync();

                var existingNews = await _newsService.GetAllAsync();
                var existingNewsVM = _mapper.Map<List<NewsVM>>(existingNews);

                ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
                ViewBag.ExistingNews = existingNewsVM;

                return View(model);
            }
                

            var entity = _mapper.Map<News>(model);
            entity.IsDeleted = false;
            entity.IsActive = true;
            entity.Date = model.Date;
            entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

            await _newsService.AddAsync(entity);


            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
                foreach (var image in model.UploadedImages)
                {
                    var relativePath = await _fileStorageService.UploadImageAsync(image, "NewsImages");

                    if (relativePath != null)
                    {
                        await _newsImagesService.AddAsync(new NewsImage
                        {
                            NewsId = entity.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });
                    }
                }

            }
            TempData["Success"] = "تمت الاضافة بنجاح";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsService.GetByIdAsync(id,x=>x.TrainigSector, x => x.NewsImages);
   
            //var news = await _newsService.GetByIdAsync(id, n => ((News)n).NewsImages);
            if (news == null) return NotFound();

            var model = _mapper.Map<NewsVM>(news);
            var sectors = await _trainingSectorService.GetAllAsync();

            ViewBag.TrainingSectorList = new SelectList(
                sectors,
                "Id",
                "NameAr",
                news.TrainigSectorId
            );

            //ViewBag.TrainingSectorList = news.TrainigSector.NameAr;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsVM model)
        {
            if (!ModelState.IsValid) {
                var sectors = await _trainingSectorService.GetDropdownListAsync();
                ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");
                return View(model);
            }

        
            var entity = await _newsService.GetByIdAsync(model.Id);
            if (entity == null) return NotFound();

            entity.TitleAr = model.TitleAr;
            entity.TitleEn = model.TitleEn;
            entity.ShortDescriptionAr = model.ShortDescriptionAr;
            entity.ShortDescriptionEn = model.ShortDescriptionEn;
            entity.DescriptionAr = model.DescriptionAr;
            entity.DescriptionEn = model.DescriptionEn;
            entity.Date = model.Date;
            entity.TrainigSectorId = model.TrainigSectorId;
            entity.IsActive = model.IsActive;
            entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);
            //var entity = _mapper.Map<News>(model);
        

            await _newsService.UpdateAsync(entity);
           
                //if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
                //{
                //    foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
                //    {
                //        var image = await _newsImagesService.GetByIdAsync(id);
                //        if (image != null)
                //        {
                //            image.IsDeleted = true;
                //            await _newsImagesService.UpdateAsync(image);
                //        }
                //    }
                //}
            if (model.DeletedImageIds != null && model.DeletedImageIds.Any())
            {
                foreach (var id in model.DeletedImageIds.Where(x => x.HasValue).Select(x => x.Value))
                {
                    var image = await _newsImagesService.GetByIdAsync(id);
                    if (image != null)
                    {
             
                        await _fileStorageService.DeleteFileAsync(image.ImagePath);
                        await _newsImagesService.DeleteAsync(id);
                    }
                }
            }


            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
                foreach (var image in model.UploadedImages)
                {
                    var relativePath = await _fileStorageService
                  .UploadImageAsync(image, "NewsImages");

                    if (relativePath != null)
                    {
                        await _newsImagesService.AddAsync(new NewsImage
                        {
                            NewsId = model.Id,
                            ImagePath = relativePath,
                            IsActive = true,
                            IsDeleted = false,
                            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                        });
                    }
                }
                //string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/newsImage");
                //if (!Directory.Exists(uploadDir))
                //    Directory.CreateDirectory(uploadDir);
                //foreach (var image in model.UploadedImages)
                //{
                //    if (image.Length > 0 && image.ContentType.StartsWith("image/"))
                //    {
                //        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                //        var filePath = Path.Combine(uploadDir, fileName);

                //        using (var stream = new FileStream(filePath, FileMode.Create))
                //        {
                //            await image.CopyToAsync(stream);
                //        }

                //        var newsImage = new NewsImage
                //        {
                //            NewsId = model.Id,
                //            ImagePath = "/uploads/newsImage/" + fileName,
                //            IsActive = true,
                //            IsDeleted = false,
                //            UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                //        };

                //        await _newsImagesService.AddAsync(newsImage);
                //    }
                //}
            }


            TempData["Success"] = "تم التعديل بنجاح";

            return RedirectToAction(nameof(Index));
        }
   
        public async Task<IActionResult> AddImages(int id)
        {
            var news = await _newsService.GetByIdAsync(id);
            if (news == null)
                return NotFound();

            var vm = new NewsImageVM
            {
                NewsId = id,
                TitleAr=news.TitleAr
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(NewsImageVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var news = await _newsService.GetByIdAsync(model.NewsId);
            if (news == null)
                return NotFound();


            foreach (var image in model.UploadedImages)
            {
                var relativePath = await _fileStorageService
              .UploadImageAsync(image, "NewsImages");

                if (relativePath != null)
                {
                    await _newsImagesService.AddAsync(new NewsImage
                    {
                        NewsId = model.NewsId,
                        ImagePath = relativePath,
                        IsActive = true,
                        IsDeleted = false,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                    });
                }
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _newsImagesService.GetByIdAsync(id);
            if (image == null) return NotFound();

            // Delete physical file
            await _fileStorageService.DeleteFileAsync(image.ImagePath);

            await _newsImagesService.DeleteAsync(id);
            return RedirectToAction("Edit", new { id = image.NewsId });

       
        }
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsService.GetByIdAsync(id);
            if (news == null) return NotFound();

            // Delete associated images from file system
            if (news.NewsImages != null && news.NewsImages.Any())
            {
                foreach (var img in news.NewsImages)
                    await _fileStorageService.DeleteFileAsync(img.ImagePath);
            }

            await _newsService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }
       
    }
}


