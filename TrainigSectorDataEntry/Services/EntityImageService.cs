using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.Services
{
    public class EntityImageService : IEntityImageService
    {
        private readonly IGenericService<EntityImage> _imageService;
        private readonly IFileStorageService _fileStorage;

        public EntityImageService(
            IGenericService<EntityImage> imageService,
            IFileStorageService fileStorage)
        {
            _imageService = imageService;
            _fileStorage = fileStorage;
        }
        public async Task AddImagesAsync(string entityType,int entityId,IEnumerable<IFormFile> files)
        {
            if (files == null || !files.Any())
                return;

            var uploadedPaths = new List<string>();

            try
            {
                foreach (var file in files)
                {
                    var path = await _fileStorage.UploadImageAsync(file, entityType);

                    if (string.IsNullOrEmpty(path))
                        throw new Exception("Image upload failed");

                    uploadedPaths.Add(path);

                    await _imageService.AddAsync(new EntityImage
                    {
                        EntityType = entityType,
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
