using ClientNexus.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Content;

public class DocumentConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Content).HasColumnType("nvarchar(2000)").IsRequired();

        // builder.Property(d => d.Type).IsRequired().HasConversion<string>();

        builder.Property(d => d.Title).HasColumnType("nvarchar(100)").IsRequired();

        builder.Property(d => d.Url).HasColumnType("varchar(500)").IsRequired();

        builder
            .HasMany(d => d.Categories)
            .WithMany(c => c.Documents)
            .UsingEntity<DocumentCategory>(
                j =>
                    j.HasOne(dc => dc.Category)
                        .WithMany(c => c.DocumentCategories)
                        .HasForeignKey(dc => dc.DCategoryId)
                        .OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne(dc => dc.Document)
                        .WithMany(d => d.DocumentCategories)
                        .HasForeignKey(dc => dc.DocumentId)
                        .OnDelete(DeleteBehavior.Cascade)
            );

        builder
            .HasOne(d => d.DocumentType)
            .WithMany(dt => dt.Documents)
            .HasForeignKey(d => d.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(d => d.UploadedBy)
            .WithMany(d => d.UploadedDocuments)
            .HasForeignKey(d => d.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
