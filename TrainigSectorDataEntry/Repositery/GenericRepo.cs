using Microsoft.EntityFrameworkCore;
using TrainigSectorDataEntry.DataContext;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.Interface;
using System.Linq.Expressions;

namespace TrainigSectorDataEntry.Repositery
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private readonly TrainingSectorDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepo(TrainingSectorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _dbSet.AsQueryable();

            if (!includeDeleted && typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool?>(e, "IsDeleted") != true);
            }

            return await query.ToListAsync();
        }

      

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null && typeof(T).GetProperty("IsDeleted") != null)
            {
                typeof(T).GetProperty("IsDeleted")!.SetValue(entity, true);
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivateAsync(int id)
        {
            await SetActiveStatus(id, true);
        }

        public async Task DeactivateAsync(int id)
        {
            await SetActiveStatus(id, false);
        }

        private async Task SetActiveStatus(int id, bool isActive)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null && typeof(T).GetProperty("IsActive") != null)
            {
                typeof(T).GetProperty("IsActive")!.SetValue(entity, isActive);
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetDropdownListAsync()
        {
         
            IQueryable<T> query = _dbSet;

            // Filter out deleted if property exists
            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == true);
            }

            // Filter by IsActive if property exists
            if (typeof(T).GetProperty("IsActive") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);
            }

            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false,params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (!includeDeleted && typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool?>(e, "IsDeleted") != true);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }


    }
}
