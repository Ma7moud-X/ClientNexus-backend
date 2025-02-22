using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Services;

public class EmergencyCategoryConfig : IEntityTypeConfiguration<EmergencyCategory>
{
    public void Configure(EntityTypeBuilder<EmergencyCategory> builder)
    {
        builder.ToTable("EmergencyCategories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        builder
            .HasOne(ec => ec.ServiceProviderType)
            .WithMany(spt => spt.EmergencyCategories)
            .HasForeignKey(ec => ec.ServiceProviderTypeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(ec => ec.EmergencyCases)
            .WithOne(ec => ec.EmergencyCategory)
            .HasForeignKey(ec => ec.EmergencyCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
