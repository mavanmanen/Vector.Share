using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vector.Share.Data;
using Vector.Share.Data.Models;

namespace Vector.Share.Repositories
{
    public interface IUploadedFileRepository : IRepository<UploadedFile, string>
    {

    }

    public class UploadedFileRepository : IUploadedFileRepository
    {
        private readonly DatabaseContext _context;

        public UploadedFileRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UploadedFile[]> GetAsync()
        {
            return await _context.UploadedFiles.ToArrayAsync();
        }

        public async Task<UploadedFile> FindAsync(string key)
        {
            return await _context.UploadedFiles.FindAsync(key);
        }

        public async Task<UploadedFile[]> FindMultipleAsync(string[] keys)
        {
            UploadedFile[] result = await Task.WhenAll(keys.Select(async key => await FindAsync(key)));
            return result.Where(entity => entity != null).ToArray();
        }

        public UploadedFile[] FindWhere(Func<UploadedFile, bool> predicate)
        {
            return _context.UploadedFiles.Where(predicate).ToArray();
        }

        public async Task DeleteAsync(UploadedFile entity)
        {
            _context.UploadedFiles.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMultipleAsync(UploadedFile[] entities)
        {
            _context.UploadedFiles.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(UploadedFile entity)
        {
            await _context.UploadedFiles.AddRangeAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}
