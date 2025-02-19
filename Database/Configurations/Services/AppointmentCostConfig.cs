using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Database.Models.Services;

namespace Database.Configurations.Services;

public class AppointmentCostConfig : IEntityTypeConfiguration<AppointmentCost>
{
    public void Configure(EntityTypeBuilder<AppointmentCost> builder)
    {
        builder.ToTable("AppointmentCosts");

        builder.HasKey(ac => new { ac.ServiceProviderId, ac.AppointmentType });

        builder.Property(ac => ac.Cost)
            .HasColumnType("decimal(10, 2)")
            .IsRequired();

        builder.Property(ac => ac.AppointmentType).HasColumnType("char(1)");

        builder.Property(ac => ac.AppointmentType).HasConversion(at => (char)at, at => (AppointmentType)at);

        builder.HasOne(ac => ac.ServiceProvider)
            .WithMany(sp => sp.AppointmentCosts)
            .HasForeignKey(ac => ac.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
