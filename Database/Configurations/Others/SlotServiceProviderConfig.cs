using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Services
{
    public class SlotServiceProviderConfig : IEntityTypeConfiguration<SlotServiceProvider>
    {
        public void Configure(EntityTypeBuilder<SlotServiceProvider> builder)
        {
            builder.ToTable("SlotsServiceProviders");

            builder.HasKey(ssp => new { ssp.SlotId, ssp.ServiceProviderId });

            builder.HasOne(ssp => ssp.Slot)
                .WithMany(s => s.SlotServiceProviders)
                .HasForeignKey(ssp => ssp.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ssp => ssp.ServiceProvider)
                .WithMany(sp => sp.SlotServiceProviders)
                .HasForeignKey(ssp => ssp.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}