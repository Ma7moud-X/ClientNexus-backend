using ClientNexus.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Content;

public class CategoryConfig : IEntityTypeConfiguration<DCategory>
{
    public void Configure(EntityTypeBuilder<DCategory> builder)
    {
        builder.ToTable("DCategories");
        
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
