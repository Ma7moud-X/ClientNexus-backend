using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Services;

public class AppointmentCostConfig : IEntityTypeConfiguration<AppointmentCost>
{
    public void Configure(EntityTypeBuilder<AppointmentCost> builder)
    {
        builder.ToTable("AppointmentCosts");

        builder.HasKey(ac => new { ac.ServiceProviderId, ac.AppointmentType });

        builder.Property(ac => ac.Cost).HasColumnType("decimal(10, 2)").IsRequired();

        builder
            .Property(ac => ac.AppointmentType)
            .HasColumnType("char(1)")
            .HasConversion(at => (char)at, at => (AppointmentType)at);

        builder
            .HasOne(ac => ac.ServiceProvider)
            .WithMany(sp => sp.AppointmentCosts)
            .HasForeignKey(ac => ac.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
