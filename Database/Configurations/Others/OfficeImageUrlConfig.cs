using Database.Models.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Others;

public class OfficeImageUrlConfig : IEntityTypeConfiguration<OfficeImageUrl>
{
    public void Configure(EntityTypeBuilder<OfficeImageUrl> builder)
    {
        builder.ToTable("OfficeImageUrls");

        builder.HasKey(oiu => oiu.Id);

        builder.Property(oiu => oiu.Url).IsRequired().HasMaxLength(1000);

        builder.Property(oiu => oiu.Id).HasConversion(id => id.ToGuid(), id => new Ulid(id));

        builder
            .HasOne(oiu => oiu.ServiceProvider)
            .WithMany(sp => sp.OfficeImageUrls)
            .HasForeignKey(oiu => oiu.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
