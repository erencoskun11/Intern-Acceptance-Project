using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            // Varsayim: entity'lerin PK'si "Id"
            // Eğer farklıysa, FindAsync kullanılabilir ama Include ile Find kullanamazsınız.
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.PropertyOrField(param, "Id");
            var lambda = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(prop, Expression.Constant(id)), param);

            return await query.AsNoTracking().FirstOrDefaultAsync(lambda);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            // Varsayim: Id property var
            var entity = await GetByIdAsync(id);
            if (entity == null) return;
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
