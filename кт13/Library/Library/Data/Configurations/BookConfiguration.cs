using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Models;

namespace Library.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Genre)
                   .HasMaxLength(50)
                   .HasDefaultValue("Не указан");

            builder.Property(b => b.Isbn)
                   .HasMaxLength(20);

            // ISBN уникален (пустые значения допускаются)
            builder.HasIndex(b => b.Isbn).IsUnique();

            builder.HasIndex(b => b.Title);
        }
    }
}
