using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        /// <summary>
        /// SaveChangesAsync'i repository seviyesinde sunmak isterseniz kullanabilirsiniz.
        /// Genelde unit-of-work veya DbContext tarafında çağrılır.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
