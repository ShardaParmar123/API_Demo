using Demo.Infrastructure.Configuration.ModelConfiguration.Auth;
using Demo.Types.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Infrastructure.Configuration
{
    public class DemoDbContext : DbContext
    {
        public readonly IConfiguration _config; 
        public DemoDbContext(DbContextOptions<DemoDbContext> options, IConfiguration config) : base(options) 
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"), opt =>
            {
                opt.MigrationsHistoryTable("_GlobleMigrationHistory", "base");
            });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AuthModelConfiguration.ApplyConfiguration(modelBuilder);
        }

        //Table 

        #region Auth

        public DbSet<User>Users { get; set; }

        #endregion
    }
}
