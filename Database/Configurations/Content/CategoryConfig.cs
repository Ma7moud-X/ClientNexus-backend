using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey( d => new {d.Id, d.DocumentId});

        builder.Property(c => c.Name).HasColumnType("nvarchar(100)").IsRequired();

        builder
            .HasOne(d => d.Document)
            .WithMany(c => c.Categories)
            .HasForeignKey(d => d.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
