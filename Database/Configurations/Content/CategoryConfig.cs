using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasColumnType("nvarchar(100)")
            .IsRequired(true);

        // builder
        //     .HasMany(c => c.DocumentsCategories)
        //     .WithOne(c => c.Category)
        //     .HasForeignKey(c => c.CategoryId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
