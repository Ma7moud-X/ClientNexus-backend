using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired(false).HasMaxLength(100);

            builder.Property(s => s.Description).IsRequired(false).HasMaxLength(500);

            builder.Property(s => s.ServiceType).HasConversion(st => (char)st, st => (ServiceType)st).HasColumnType("varchar(1)").IsRequired();

            builder
                .Property(s => s.Status)
                .HasConversion(st => (char)st, st => (ServiceStatus)st)
                .IsRequired();

            builder.Property(s => s.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");

            builder
                .HasOne(s => s.Client)
                .WithMany(s => s.Services)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(s => s.ServiceProvider)
                .WithMany(sv => sv.ServicesProvided)
                .HasForeignKey(s => s.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
