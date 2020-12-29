using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vector.Share.Data;
using Vector.Share.Data.Models;

namespace Vector.Share.Repositories
{
    public interface IScheduledDeletionRepository : IRepository<ScheduledDeletion, string>
    {

    }

    public class ScheduledDeletionRepository : IScheduledDeletionRepository
    {
        private readonly DatabaseContext _context;

        public ScheduledDeletionRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ScheduledDeletion[]> GetAsync()
        {
            return await _context.ScheduledDeletions.ToArrayAsync();
        }

        public async Task<ScheduledDeletion> FindAsync(string key)
        {
            return await _context.ScheduledDeletions.FindAsync(key);
        }

        public async Task<ScheduledDeletion[]> FindMultipleAsync(string[] keys)
        {
            ScheduledDeletion[] result = await Task.WhenAll(keys.Select(async key => await FindAsync(key)));
            return result.Where(entity => entity != null).ToArray();
        }

        public ScheduledDeletion[] FindWhere(Func<ScheduledDeletion, bool> predicate)
        {
            return _context.ScheduledDeletions.Where(predicate).ToArray();
        }

        public async Task DeleteAsync(ScheduledDeletion entity)
        {
            _context.ScheduledDeletions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMultipleAsync(ScheduledDeletion[] entities)
        {
            _context.ScheduledDeletions.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(ScheduledDeletion entity)
        {
            await _context.ScheduledDeletions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}
