using TrainigSectorDataEntry.Enities;

namespace TrainigSectorDataEntry.Interface
{
    public interface IGeneric<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity, IFormFile? imageFile = null);
        Task UpdateAsync(T entity, IFormFile? imageFile = null);
        Task SoftDeleteAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}
