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

            builder.Property(a => a.AppointmentType).HasConversion(at => (char)at, at => (AppointmentType)at).IsRequired().HasColumnType("varchar(1)");

            builder.Property(a => a.Date).IsRequired();

            builder.Property(a => a.Price).IsRequired().HasColumnType("decimal(18,2)");
        }
    }
}
