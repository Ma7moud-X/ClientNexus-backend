
using ClientNexus.Domain.Entities.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Others
{
    public class AvailableDayConfig : IEntityTypeConfiguration<AvailableDay>

    {
        public void Configure(EntityTypeBuilder<AvailableDay> builder)
        {
            builder.ToTable("AvailableDays");

            builder.HasKey(ad => ad.Id);

            builder.Property(ad => ad.Id).ValueGeneratedOnAdd();

            builder.Property(ad => ad.DayOfWeek).IsRequired();

            builder.Property(ad => ad.StartTime).IsRequired();

            builder.Property(ad => ad.EndTime).IsRequired();

            builder.Property(ad => ad.SlotDuration).IsRequired();

            builder.Property(ad => ad.SlotType).IsRequired();

            // Configure foreign key relationship with ServiceProvider
            builder.HasOne(ad => ad.ServiceProvider)
                .WithMany(sp => sp.AvailableDays)
                .HasForeignKey(ad => ad.ServiceProviderId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
    
}
