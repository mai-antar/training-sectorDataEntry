using System.Linq.Expressions;
using TrainigSectorDataEntry.Models;

namespace TrainigSectorDataEntry.Interface
{

    public interface IGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<IEnumerable<T>> GetDropdownListAsync();
         Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false,params Expression<Func<T, object>>[] includes);


    }
}
