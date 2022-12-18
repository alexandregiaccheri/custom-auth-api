using CustomAuthApi.Data.Context;
using CustomAuthApi.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomAuthApi.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CustomAuthDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(CustomAuthDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return;
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _dbSet.Where(filter);
            return await query.FirstOrDefaultAsync();
        }
    }
}
