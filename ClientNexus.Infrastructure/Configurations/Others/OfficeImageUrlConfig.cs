using ClientNexus.Domain.Entities.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Others;

public class OfficeImageUrlConfig : IEntityTypeConfiguration<OfficeImageUrl>
{
    public void Configure(EntityTypeBuilder<OfficeImageUrl> builder)
    {
        builder.ToTable("OfficeImageUrls");

        builder.HasKey(oiu => new { oiu.ServiceProviderId, oiu.Id });

        builder.Property(oiu => oiu.Url).IsRequired().HasMaxLength(1000);

        builder.Property(oiu => oiu.Id).HasConversion(id => id.ToGuid(), id => new Ulid(id));

        builder
            .HasOne(oiu => oiu.ServiceProvider)
            .WithMany(sp => sp.OfficeImageUrls)
            .HasForeignKey(oiu => oiu.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
