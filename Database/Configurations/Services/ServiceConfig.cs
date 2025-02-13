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

            builder.Property(s => s.Name)
                .IsRequired(true)
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.Status)
                .HasDefaultValue(ServiceStatus.Pending)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder
                .HasOne(s => s.Client)
                .WithMany(s => s.Services)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}