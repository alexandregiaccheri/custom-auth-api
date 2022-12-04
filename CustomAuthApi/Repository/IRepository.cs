using System.Linq.Expressions;

namespace CustomAuthApi.Repository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);
    }
}
