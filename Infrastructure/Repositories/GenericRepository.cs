using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {            
            IQueryable<T> query = _dbSet;

            // Get the name of the entity's primary column (Primary Key)
            var keyProperty = _context.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault(); // safe: returns null if no PK


            if (keyProperty != null)
            {
                // Dynamic descending order by Primary Key
                query = query.OrderByDescending(x => EF.Property<object>(x, keyProperty.Name));
            }
            else
            {
                // No primary key — just return as-is or add a default OrderByDescending with First Column
                // e.g., by first property (optional)
                //query = query.OrderByDescending(x => 0); // keeps IQueryable valid without this sorting
            }

            return await query.ToListAsync();
        }

        public async Task<bool> CheckTableIfEmpty()
        {
            var element = await _dbSet.FirstOrDefaultAsync();
            return (element == null);
        }
        
        public IQueryable<T> Table => _context.Set<T>();

        public IQueryable<T> GetQueryable() => _context.Set<T>();
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Get the name of the entity's primary column (Primary Key)
            var keyProperty = _context.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault(); // safe: returns null if no PK

            if (keyProperty != null)
            {
                // Dynamic descending order by Primary Key
                query = query.OrderByDescending(x => EF.Property<object>(x, keyProperty.Name));
            }
            else
            {
                // No primary key — just return as-is or add a default OrderByDescending with First Column
                // e.g., by first property (optional)
                //query = query.OrderByDescending(x => 0); // keeps IQueryable valid without this sorting
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply includes
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply predicate
            query = query.Where(predicate);

            // Get the name of the entity's primary column (Primary Key)
            var keyProperty = _context.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault(); // safe: returns null if no PK

            if (keyProperty != null)
            {
                // Dynamic descending order by Primary Key
                query = query.OrderByDescending(x => EF.Property<object>(x, keyProperty.Name));
            }
            else
            {
                // No primary key — just return as-is or add a default OrderByDescending with First Column
                // e.g., by first property (optional)
                //query = query.OrderByDescending(x => 0); // keeps IQueryable valid without this sorting
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T?> GetByColumnAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public async Task<T?> GetByColumnAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task<T?> GetByColumnWithIncludesAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeFunc)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeFunc(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetMaxRecordAsync<TKey>(
    Expression<Func<T, TKey>> keySelector,
    params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            // Check if there are elements before MaxAsync works
            if (!await query.AnyAsync())
            {
                return null;
            }

            var maxValue = await query.MaxAsync(keySelector);

            // Build expression to filter by max value
            var parameter = Expression.Parameter(typeof(T), "x");
            var equal = Expression.Equal(
                Expression.Invoke(keySelector, parameter),
                Expression.Constant(maxValue)
            );
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        public async Task AddAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
                await _dbSet.AddAsync(entity);
        }

        public async Task<int> AddAsyncThenGetLastId(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return await _dbSet
                .OrderByDescending(e => EF.Property<int>(e, "Id"))
                .Select(e => EF.Property<int>(e, "Id"))
                .FirstOrDefaultAsync();
        }


        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void UpdateValues<TEntity>(TEntity trackedEntity, TEntity newValues) where TEntity : class
        {
            _context.Entry(trackedEntity).CurrentValues.SetValues(newValues);
        }
        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public IQueryable<T> GetAsync(Expression<Func<T, bool>> filter = null,params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
                query = query.Where(filter);

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return query;
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

    }

}
