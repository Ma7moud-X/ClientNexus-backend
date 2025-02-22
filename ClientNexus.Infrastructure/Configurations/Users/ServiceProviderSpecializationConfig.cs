using ClientNexus.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Users;

public class ServiceProviderSpecializationConfig : IEntityTypeConfiguration<ServiceProviderSpecialization>
{
    public void Configure(EntityTypeBuilder<ServiceProviderSpecialization> builder)
    {
        builder.ToTable("ServiceProviderSpecializations");

        builder.HasKey(sps => new {sps.ServiceProviderId, sps.SpecializationId});
    }
}
