using Microsoft.EntityFrameworkCore;

namespace UserManagement
{
    public class UserDbContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>(); 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                   => optionsBuilder.UseNpgsql("Server = 127.0.0.1; Port = 5432; Database = easyidentity; User Id = postgres; Password = admin");
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEntity>()
                .HasKey(u => new { u.ProjectId, u.UserId });

            builder.Entity<UserEntity>()
                .HasIndex(u => new { u.ProjectId, u.EmailAddress })
                .IsUnique();

            builder.Entity<UserEntity>()
                .Property(u => u.Username)
                .IsRequired(false);
        }
    }
}