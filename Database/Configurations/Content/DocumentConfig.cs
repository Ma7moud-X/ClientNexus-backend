using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content;

public class DocumentConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        // data types configuration
        builder.Property(d => d.Title).HasColumnType("nvarchar(100)").IsRequired();
        builder.Property(d => d.Url).HasColumnType("varchar(500)").IsRequired();

        // relationships configuration
        builder
            .HasOne(d => d.Category)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(d => d.UploadedBy)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
