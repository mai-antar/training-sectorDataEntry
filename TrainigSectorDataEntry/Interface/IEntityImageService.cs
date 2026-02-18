using System.Linq.Expressions;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.Interface
{

    public interface IEntityImageService
    {
        Task AddImagesAsync(int entityType,int entityId,IEnumerable<IFormFile> files);

        Task DeleteImageAsync(int imageId);
        Task<List<EntityImage>> FindAsync(Expression<Func<EntityImage, bool>> predicate);
        Task<EntityImage?> GetByIdAsync(int id);
    }
}
