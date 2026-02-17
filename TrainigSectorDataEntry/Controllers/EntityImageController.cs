using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.Services;
using TrainigSectorDataEntry.ViewModel;

namespace TrainigSectorDataEntry.Controllers
{
    public class EntityImageController : Controller
    {
       
        private readonly IFileStorageService _fileStorageService;
        private readonly IEntityImageService _entityImageService;

        public EntityImageController(
            IEntityImageService entityImageService,
            IFileStorageService fileStorageService)
        {
            _entityImageService = entityImageService;
            _fileStorageService = fileStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(EntityImageVM model)
        {
            await _entityImageService.AddImagesAsync(model.EntityType, model.EntityId,model.UploadedImages);

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
