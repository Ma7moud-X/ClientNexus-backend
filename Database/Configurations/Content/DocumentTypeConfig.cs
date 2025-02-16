using Database.Models.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Content;

public class DocumentTypeConfig : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.ToTable("DocumentTypes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnType("nvarchar(50)").IsRequired();

    }
}
