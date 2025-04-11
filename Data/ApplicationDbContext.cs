using Microsoft.EntityFrameworkCore;
using InteriorBackend.Models;

namespace InteriorBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }


        public DbSet<UserSave> UserSaves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserSave>()
    .Property(e => e.ImageUrlsJson)
    .HasColumnName("ImageUrls");


        }
    }
}
