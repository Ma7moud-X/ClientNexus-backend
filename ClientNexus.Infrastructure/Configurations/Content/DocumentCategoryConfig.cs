using ClientNexus.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Content
{
    public class DocumentCategoryConfig : IEntityTypeConfiguration<DocumentCategory>
    {
        public void Configure(EntityTypeBuilder<DocumentCategory> builder)
        {
            builder.ToTable("DocumentCategories");

            builder.HasKey(dc => new { dc.DocumentId, dc.DCategoryId });

            //builder.HasOne(dc => dc.Document)
            //    .WithMany(dc => dc.DocumentCategories)
            //    .HasForeignKey(dc => dc.DocumentId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(dc => dc.Category)
            //    .WithMany(dc => dc.DocumentCategories)
            //    .HasForeignKey(dc => dc.DCategoryId)
            //    .OnDelete(DeleteBehavior.Cascade);

  

        }
    }
}
