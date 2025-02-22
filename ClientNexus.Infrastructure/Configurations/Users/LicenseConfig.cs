using ClientNexus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class LicenseConfig : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.ToTable("Licenses");

            builder.HasKey(l => new { l.ServiceProviderId, l.Id });

            builder.Property(l => l.Id).UseIdentityColumn();

            builder.Property(l => l.LicenceNumber).IsRequired(true).HasMaxLength(50);

            builder.Property(l => l.IssuingAuthority).IsRequired(true).HasMaxLength(100);

            builder.Property(l => l.IssueDate).IsRequired(true);

            builder.Property(l => l.ExpiryDate).IsRequired(true);

            builder.Property(l => l.ImageUrl).HasColumnType("nvarchar(500)");

            builder
                .HasOne(l => l.ServiceProvider)
                .WithMany(p => p.Licenses)
                .HasForeignKey(l => l.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
