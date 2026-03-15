using System.Linq.Expressions;

namespace Infrastructure.Repositories.InterfacesDB
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Table { get; }
        IQueryable<T> GetQueryable();
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> CheckTableIfEmpty();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetByColumnAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByColumnAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByColumnWithIncludesAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeFunc);
        Task<T?> GetMaxRecordAsync<TKey>(Expression<Func<T, TKey>> keySelector, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task AddAsync(IEnumerable<T> entities);
        Task<int> AddAsyncThenGetLastId(T entity);
        void Update(T entity);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void UpdateValues<TEntity>(TEntity trackedEntity, TEntity newValues) where TEntity : class;
        Task<bool> SaveChangesAsync();
        IQueryable<T> GetAsync(Expression<Func<T, bool>> filter = null,params Expression<Func<T, object>>[] includes);
        bool Any(Expression<Func<T, bool>> predicate);
    }
}
