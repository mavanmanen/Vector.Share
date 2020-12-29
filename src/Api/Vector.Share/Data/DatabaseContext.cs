using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vector.Share.Data.Models;

namespace Vector.Share.Data
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("Default"));
        }

        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<ScheduledDeletion> ScheduledDeletions { get; set; }
    }
}
