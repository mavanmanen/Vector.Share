using System;
using System.Threading.Tasks;

namespace Vector.Share.Repositories
{
    public interface IRepository<TEntity, in TKey>
    {
        Task<TEntity[]> GetAsync();
        Task<TEntity> FindAsync(TKey key);
        Task<TEntity[]> FindMultipleAsync(TKey[] keys);
        TEntity[] FindWhere(Func<TEntity, bool> predicate);
        Task DeleteAsync(TEntity entity);
        Task DeleteMultipleAsync(TEntity[] entities);
        Task AddAsync(TEntity entity);
    }
}
