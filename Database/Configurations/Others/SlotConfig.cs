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

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Date).IsRequired();

            builder
                .Property(s => s.Status)
                .HasColumnType("varchar(1)")
                .HasConversion(
                    status => (char)status,
                    status => (SlotStatus)status
                )
                .IsRequired();

            builder
                .HasMany(s => s.Appointments)
                .WithOne(s => s.Slot)
                .HasForeignKey(s => s.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(s => s.ServiceProvider)
                .WithMany(sp => sp.Slots)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.SlotType).HasConversion(
                type => (char)type,
                type => (SlotType)type
            ).HasColumnType("varchar(1)").IsRequired();
        }
    }
}
