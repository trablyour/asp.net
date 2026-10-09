using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Models;

namespace Library.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Country)
                   .HasMaxLength(60);

            builder.Property(a => a.Biography)
                   .HasMaxLength(1000);

            // FullName вычисляется в коде, в базе его нет
            builder.Ignore(a => a.FullName);

            builder.HasIndex(a => new { a.LastName, a.FirstName });

            // один ко многим: автор -> книги, при удалении автора удаляются его книги
            builder.HasMany(a => a.Books)
                   .WithOne(b => b.Author)
                   .HasForeignKey(b => b.AuthorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
