using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey( c => new {c.Id, c.DocumentId});

        builder.Property(c => c.Name)
            .HasColumnType("nvarchar(100)")
            .IsRequired(true);

        builder
            .HasOne(c => c.Document)
            .WithMany(c => c.Categories)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
