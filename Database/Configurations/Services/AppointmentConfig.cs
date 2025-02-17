using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Services
{
    public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasBaseType<Service>();

            builder.Property(a => a.Type)
                .IsRequired();

            builder.Property(a => a.Date)
                .IsRequired();
                
            builder.Property(a => a.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            builder
                .HasOne(a => a.Slot)
                .WithOne(a => a.Appointment)
                .HasForeignKey<Appointment>(a => a.SlotId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}