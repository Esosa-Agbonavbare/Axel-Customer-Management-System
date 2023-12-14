using AxelCMS.Application.Interfaces.Repositories;
using AxelCMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AxelCMS.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbSet<T> _entities;
        protected readonly AxelCMSDbContext _dbContext;

        public GenericRepository(AxelCMSDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _entities = _dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _entities.AnyAsync(predicate);

        public async Task<IList<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _entities.Where(predicate).ToListAsync();

        public async Task<IList<T>> GetAllAsync() => await _entities.ToListAsync();

        public async Task<T> GetByIdAsync(string id) => await _entities.FindAsync(id);

        public async Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
