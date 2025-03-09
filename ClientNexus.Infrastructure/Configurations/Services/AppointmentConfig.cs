using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Services
{
    public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasBaseType<Service>();

            builder.Property(a => a.AppointmentType).HasConversion(at => (char)at, at => (AppointmentType)at).IsRequired().HasColumnType("char(1)");

            builder.Property(a => a.SlotId).IsRequired();

            //builder.Property(a => a.Date).IsRequired();
        }
    }
}
