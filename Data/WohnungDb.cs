using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class WohnungDb: DbContext
    {
        public virtual DbSet<WohnungHeader> WohnungHeaders { get; set; }
        public virtual DbSet<WohnungDetails> WohnungDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Data\\WohnungDb.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WohnungHeader>().HasIndex(p => new { p.Provider, p.WohnungId }).IsUnique();
        }
    }
}
