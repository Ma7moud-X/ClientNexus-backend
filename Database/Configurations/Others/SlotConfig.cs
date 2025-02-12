using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Services
{
    public class SlotConfig : IEntityTypeConfiguration<Slot>
    {
        public void Configure(EntityTypeBuilder<Slot> builder)
        {
            builder.ToTable("Slots");

            builder.ToTable("Slots");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Date)
                .IsRequired();

            // Configure one-to-one with Appointment
            builder.HasOne(s => s.Appointment)
                .WithOne(s => s.Slot)
                .HasForeignKey<Appointment>(s => s.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many with SlotTypes
            builder.HasMany(s => s.SlotTypes)
                .WithOne(s => s.Slot)
                .HasForeignKey(s => s.SlotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add many-to-many relationship with ServiceProvider
            builder.HasMany(s => s.SlotServiceProviders)
                .WithOne(s => s.Slot)
                .HasForeignKey(s => s.SlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}