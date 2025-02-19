using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content
{
    public class DocumentCategoryConfig : IEntityTypeConfiguration<DocumentCategory>
    {
        public void Configure(EntityTypeBuilder<DocumentCategory> builder)
        {
            builder.ToTable("DocumentCategories");

            builder.HasKey(dc => new { dc.DocumentId, dc.DCategoryId });

            // builder.HasOne(dc => dc.Document)
            //     .WithMany(dc => dc.DocumentsCategories)
            //     .HasForeignKey(dc => dc.DocumentId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.HasOne(dc => dc.Category)
            //     .WithMany(dc => dc.DocumentsCategories)
            //     .HasForeignKey(dc => dc.CategoryId)
            //     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
