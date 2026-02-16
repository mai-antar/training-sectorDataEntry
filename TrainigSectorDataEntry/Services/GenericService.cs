using System.Linq.Expressions;
using TrainigSectorDataEntry.Interface;

namespace TrainigSectorDataEntry.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepo<T> _repository;

        public GenericService(IGenericRepo<T> repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false) =>
            _repository.GetAllAsync(includeDeleted);

        public Task<T?> GetByIdAsync(int id) =>
            _repository.GetByIdAsync(id);

        public Task AddAsync(T entity) =>
            _repository.AddAsync(entity);

        public Task UpdateAsync(T entity) =>
            _repository.UpdateAsync(entity);

        public Task DeleteAsync(int id) =>
            _repository.DeleteAsync(id);

        public Task ActivateAsync(int id) =>
            _repository.ActivateAsync(id);

        public Task DeactivateAsync(int id) =>
            _repository.DeactivateAsync(id);
        public Task<IEnumerable<T>> GetDropdownListAsync()
        {
            
            return _repository.GetAllAsync(includeDeleted: false);
        }
        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            return await _repository.GetByIdAsync(id, includes);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false,params Expression<Func<T, object>>[] includes)
        {
            return await _repository.GetAllAsync(includeDeleted, includes);
        }
        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync( predicate);
        }
    }
}

