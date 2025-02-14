using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class AddressConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.Property(a => a.DetailedAddress)
                .IsRequired(true)
                .HasMaxLength(200);

            builder.Property(a => a.Neighborhood)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(a => a.City)
                .IsRequired(true)
                .HasMaxLength(20);
            
            builder.Property(a => a.MapUrl)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)");

            builder
                .HasOne(sp => sp.ServiceProvider)
                .WithMany(a => a.Addresses)
                .HasForeignKey(sp => sp.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}