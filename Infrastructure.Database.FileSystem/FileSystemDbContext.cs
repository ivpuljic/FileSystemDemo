using Infrastructure.Database.FileSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.FileSystem
{
    public class FileSystemDbContext : DbContext
    {
        public FileSystemDbContext(DbContextOptions<FileSystemDbContext> options) : base(options)
        {
        }

        public DbSet<FileDb> FileDb { get; set; }
        public DbSet<PathDb> PathDb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PathDb>().HasKey(x => x.Id);
            modelBuilder.Entity<PathDb>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<PathDb>()
                .HasMany(x => x.Files)
                .WithOne(x => x.Path)
                .HasForeignKey(x => x.PathId)
                .IsRequired(false);

            modelBuilder.Entity<FileDb>().HasKey(x => x.Id);
            modelBuilder.Entity<FileDb>().Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
