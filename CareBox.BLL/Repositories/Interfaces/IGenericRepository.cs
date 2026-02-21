using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // 1. Basic Retrieval
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();

        // 2. Search & Filtering (with Includes for Relations)
        Task<T?> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);

        // 3. Pagination (Important for Listings, Orders, Bookings)
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int skip, int take, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take, Expression<Func<T, object>> orderBy = null, string orderByDirection = "ASC");

        // 4. Modification
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        // 5. Aggregates
        Task<int> CountAsync(Expression<Func<T, bool>> criteria = null);
        Task<bool> IsExistAsync(Expression<Func<T, bool>> criteria);
    }
}

