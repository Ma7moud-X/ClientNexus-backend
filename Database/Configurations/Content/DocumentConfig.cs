using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content;

public class DocumentConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Content)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.Title)
            .HasColumnType("nvarchar(100)")
            .IsRequired();
        
        builder.Property(d => d.Url)
            .HasColumnType("varchar(500)")
            .IsRequired();


        builder
            .HasMany(d => d.Categories)
            .WithOne(d => d.Document)
            .HasForeignKey(d => d.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(d => d.UploadedBy)
            .WithMany(d => d.Documents)
            .HasForeignKey(d => d.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
