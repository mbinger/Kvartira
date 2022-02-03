using Common;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class WohnungDb: DbContext
    {
        public static AppConfig AppConfig;

        public virtual DbSet<ProviderHealthEntity> ProviderHealthLogs { get; set; }
        public virtual DbSet<WohnungEntity> Wohnungen { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var db = AppConfig?.DatabaseFile ?? @"c:\Kvartira\Db\WohnungDb.sqlite";
            optionsBuilder.UseSqlite($"Filename={db}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WohnungEntity>().HasIndex(p => new { p.Provider, p.WohnungId }).IsUnique();
        }
    }
}
