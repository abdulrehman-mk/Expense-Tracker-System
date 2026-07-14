using Microsoft.EntityFrameworkCore;
using technova_ecommerce.Models.Entities;

namespace Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }
        public DbSet<technova_ecommerce.Models.Entities.Expense> Expense { get; set; } = default!;
    }
}
