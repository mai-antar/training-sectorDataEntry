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
using TrainigSectorDataEntry.DataContext;
using Microsoft.EntityFrameworkCore;

namespace TrainigSectorDataEntry.Controllers
{
    public class NewsController : Controller
    {
        private readonly IGenericService<News> _newsService;
        private readonly IGenericService<NewsImage> _newsImagesService;
        private readonly IEntityImageService _entityImageService;
        private readonly IGenericService<TrainingSector> _trainingSectorService;
        private readonly IMapper _mapper;
        private readonly ILoggerRepository _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService;
        private readonly TrainingSectorDbContext _context;
        public NewsController(IGenericService<News> newsService, IGenericService<NewsImage> newsImagesService,IGenericService<TrainingSector> trainingSectorService,
            IMapper mapper, ILoggerRepository logger, IConfiguration configuration, IFileStorageService fileStorageService,
            IEntityImageService entityImageService, TrainingSectorDbContext context)
        {
            _newsService = newsService;
            _newsImagesService = newsImagesService;
            _trainingSectorService = trainingSectorService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _fileStorageService = fileStorageService;
            _entityImageService = entityImageService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var newsList = await _newsService.GetAllAsync(false,x=>x.TrainigSector, x => x.NewsImages);
            var vm = _mapper.Map<List<NewsVM>>(newsList);

            var newsImagesList = await _entityImageService.FindAsync(
                     x => x.EntityImagesTableTypeId == 2 && x.IsDeleted != true);

            foreach (var item in vm)
            {
                if (newsImagesList.Where(a => a.EntityId == item.Id).ToList().Count > 0)
                {

                    item.NewsImages = newsImagesList.Where(a => a.EntityId == item.Id).ToList();
                }
            }

            return View(vm);


            
        }

        public async Task<IActionResult> Create()
        {
            var sectors = await _trainingSectorService.GetDropdownListAsync();
            ViewBag.TrainingSectorList = new SelectList(sectors, "Id", "NameAr");

            var existingNews = await _newsService.GetAllAsync();
           
            var existingNewsVM = _mapper.Map<List<NewsVM>>(existingNews);

            var newsImagesList = await _entityImageService.FindAsync(
              x => x.EntityImagesTableTypeId == 2 && x.IsDeleted != true);

            foreach (var item in existingNewsVM)
            {
                if (newsImagesList.Where(a => a.EntityId == item.Id).ToList().Count > 0)
                {

                    item.NewsImages = newsImagesList.Where(a => a.EntityId == item.Id).ToList();
                }
            }


            // Preserve selected facility
            if (TempData["News_TrainingSectorId"] != null)
            {
                ViewBag.TrainingSectorList = new SelectList(
                    sectors,
                    "Id",
                    "NameAr",
                    TempData["News_TrainingSectorId"]);
            }


            ViewBag.ExistingNews = existingNewsVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var entity = _mapper.Map<News>(model);
                entity.IsDeleted = false;
                entity.IsActive = true;
                entity.Date = model.Date;
                entity.UserCreationDate = DateOnly.FromDateTime(DateTime.Today);

                await _newsService.AddAsync(entity);


                if (model.UploadedImages != null && model.UploadedImages.Any())
                {
                    await _entityImageService.AddImagesAsync(
                        2,
                        entity.Id,
                        model.UploadedImages);
                }

                await transaction.CommitAsync();

                TempData["Success"] = "تمت الاضافة بنجاح";
                TempData["News_TrainingSectorId"] = model.TrainigSectorId;
                return RedirectToAction(nameof(Create));

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, nameof(NewsController), nameof(Create));
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ، تم إلغاء العملية.");

                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsService.GetByIdAsync(id,x=>x.TrainigSector);
            if (news == null) return NotFound();

            var model = _mapper.Map<NewsVM>(news);

            var newsImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 2 && x.EntityId == id && x.IsDeleted == false);
            model.NewsImages = newsImages;

            
            var sectors = await _trainingSectorService.GetAllAsync();

            ViewBag.TrainingSectorList = new SelectList(
                sectors,
                "Id",
                "NameAr",
                news.TrainigSectorId
            );

         
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var entity = await _newsService.GetByIdAsync(model.Id);
                if (entity == null) return NotFound();

                entity.TitleAr = model.TitleAr;
                entity.TitleEn = model.TitleEn;
                //entity.ShortDescriptionAr = model.ShortDescriptionAr;
                //entity.ShortDescriptionEn = model.ShortDescriptionEn;
                entity.DescriptionAr = model.DescriptionAr;
                entity.DescriptionEn = model.DescriptionEn;
                entity.Date = model.Date;
                entity.TrainigSectorId = model.TrainigSectorId;
                entity.IsActive = model.IsActive;
                entity.UserUpdationDate = DateOnly.FromDateTime(DateTime.Today);
                //var entity = _mapper.Map<News>(model);


                await _newsService.UpdateAsync(entity);

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
                        2,
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

                _logger.LogError(ex, nameof(ProjectsController), nameof(Edit));
                ModelState.AddModelError("", "حدث خطأ أثناء التعديل، تم إلغاء العملية.");

                return View(model);
            }
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
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsService.GetByIdAsync(id);
            if (news == null) return NotFound();

            // Delete associated images from file system
            var NewsImages = await _entityImageService.FindAsync(x => x.EntityImagesTableTypeId == 2 && x.EntityId == id && x.IsDeleted == false);

            if (NewsImages != null && NewsImages.Any())
            {
                foreach (var img in NewsImages)
                    await _fileStorageService.DeleteFileAsync(img.ImagePath);
            }

            await _newsService.DeleteAsync(id);

            TempData["Success"] = "تم الحذف بنجاح";

            return RedirectToAction(nameof(Index));
        }
       
    }
}


