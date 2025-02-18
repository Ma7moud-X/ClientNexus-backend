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

            builder.HasBaseType<Service>();

            builder.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        }
    }
}