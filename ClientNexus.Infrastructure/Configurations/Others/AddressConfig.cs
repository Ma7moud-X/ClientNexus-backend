using ClientNexus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class AddressConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => new { a.BaseUserId, a.Id });

            builder.Property(a => a.Id).UseIdentityColumn();

            builder.Property(a => a.DetailedAddress)
                .IsRequired(true)
                .HasMaxLength(200);

            //builder.Property(a => a.Neighborhood)
            //    .IsRequired(false)
            //    .HasMaxLength(100);

            builder.Property(a => a.MapUrl)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)");

            builder
                .HasOne(a => a.BaseUser)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.BaseUserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
