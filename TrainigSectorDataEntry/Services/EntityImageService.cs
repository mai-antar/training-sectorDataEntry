using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.Services
{
    public class EntityImageService : IEntityImageService
    {
        private readonly IGenericService<EntityImage> _imageService;
        private readonly IGenericService<EntityImagesTableType> _EntityImagesTableTypeService;
        private readonly IFileStorageService _fileStorage;

        public EntityImageService(
            IGenericService<EntityImage> imageService,
            IFileStorageService fileStorage, IGenericService<EntityImagesTableType> EntityImagesTableTypeService)
        {
            _imageService = imageService;
            _fileStorage = fileStorage;
            _EntityImagesTableTypeService = EntityImagesTableTypeService;
        }
        public async Task AddImagesAsync(int entityType,int entityId,IEnumerable<IFormFile> files)
        {
            if (files == null || !files.Any())
                return;

            var uploadedPaths = new List<string>();

            var tableType = await _EntityImagesTableTypeService.GetByIdAsync(entityType);

            if (tableType == null)
                throw new Exception("Invalid entity type");

            var folderName = tableType.Name.Trim(); 

            try
            {
                foreach (var file in files)
                {
                    var path = await _fileStorage.UploadImageAsync(file, folderName);

                    if (string.IsNullOrEmpty(path))
                        throw new Exception("Image upload failed");

                    uploadedPaths.Add(path);

                    await _imageService.AddAsync(new EntityImage
                    {
                        EntityImagesTableTypeId = entityType,
                        EntityId = entityId,
                        ImagePath = path,
                        IsActive = true,
                        IsDeleted = false,
                        UserCreationDate = DateOnly.FromDateTime(DateTime.Today)
                    });
                }
            }
            catch
            {
                // Cleanup uploaded files
                foreach (var path in uploadedPaths)
                {
                    await _fileStorage.DeleteFileAsync(path);
                }

                throw; // rethrow → controller transaction will rollback DB
            }
        }


        public async Task<List<EntityImage>> FindAsync(Expression<Func<EntityImage, bool>> predicate)
        {
            return await _imageService.FindAsync(predicate);
        }
        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _imageService.GetByIdAsync(imageId);
            if (image == null) return;

            await _fileStorage.DeleteFileAsync(image.ImagePath);
            await _imageService.DeleteAsync(imageId);
        }

        public async Task<EntityImage?> GetByIdAsync(int id)
        {
            return await _imageService.GetByIdAsync(id);
        }
    }

}
