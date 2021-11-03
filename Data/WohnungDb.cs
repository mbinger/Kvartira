using Common;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class WohnungDb: DbContext
    {
        public static AppConfig AppConfig;

        public virtual DbSet<WohnungHeaderEntity> WohnungHeaders { get; set; }
        public virtual DbSet<WohnungDetailsEntity> WohnungDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={AppConfig.DatabaseFile}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WohnungHeaderEntity>().HasIndex(p => new { p.Provider, p.WohnungId }).IsUnique();
        }
    }
}
