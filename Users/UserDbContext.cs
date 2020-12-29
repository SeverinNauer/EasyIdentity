using EasyIdentity.Core;
using Microsoft.EntityFrameworkCore;

namespace Users
{
    public class UserDbContext : DbContext
    {
        public DbSet<UserModel> Users => Set<UserModel>(); 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                   => optionsBuilder.UseNpgsql("Server = 127.0.0.1; Port = 5432; Database = easyidentity; User Id = postgres; Password = admin");
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserModel>()
                .HasKey(u => new { u.ProjectId, u.UserId });

            builder.Entity<UserModel>()
                .HasIndex(u => new { u.ProjectId, u.EmailAddress })
                .IsUnique();

            builder.Entity<UserModel>()
                .HasIndex(u => new { u.ProjectId, u.Username })
                .IsUnique();

            builder.Entity<UserModel>()
                .Property(u => u.Username)
                .IsRequired(false)
                .HasMaxLength(Username.MaxLength);

            builder.Entity<UserModel>()
                .Property(u => u.EmailAddress)
                .IsRequired()
                .HasMaxLength(EmailAddress.MaxLength);

            builder.Entity<UserModel>()
                .Property(u => u.Password)
                .IsRequired();
        }
    }
}