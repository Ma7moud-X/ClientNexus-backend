using ClientNexus.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Users;

public class SpecializationConfig : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("Specializations");

        builder.Property(s => s.Name).HasColumnType("nvarchar(100)").IsRequired();

        builder
            .HasOne(s => s.ServiceProviderType)
            .WithMany(s => s.Specializations)
            .HasForeignKey(s => s.ServiceProviderTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(s => s.ServiceProviders)
            .WithMany(sp => sp.Specializations)
            .UsingEntity<ServiceProviderSpecialization>(
                j =>
                    j.HasOne(sps => sps.ServiceProvider)
                        .WithMany(sp => sp.ServiceProviderSpecializations)
                        .HasForeignKey(sps => sps.ServiceProviderId)
                        .OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne(sps => sps.Specialization)
                        .WithMany(s => s.ServiceProviderSpecializations)
                        .HasForeignKey(sps => sps.SpecializationId)
                        .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
