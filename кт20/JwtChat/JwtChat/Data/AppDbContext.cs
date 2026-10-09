using Microsoft.EntityFrameworkCore;
using JwtChat.Models;

namespace JwtChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<ChatMessage> Messages => Set<ChatMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // логин без учета регистра
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .UseCollation("NOCASE");

            modelBuilder.Entity<ChatMessage>()
                .HasIndex(m => m.SentAt);
        }
    }
}
