using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class EmergencyCaseConfig : IEntityTypeConfiguration<EmergencyCase>
    {
        public void Configure(EntityTypeBuilder<EmergencyCase> builder)
        {
            builder.ToTable("EmergencyCases");

            // Configure inheritance
            builder.HasBaseType<Service>();

            // Configure required fields
            builder.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Configure relationships
            builder.HasOne(e => e.ServiceProvider)
                .WithMany(e => e.EmergencyCases)
                .HasForeignKey(e => e.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}