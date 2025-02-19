using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users;

public class ServiceProviderSpecializationConfig : IEntityTypeConfiguration<ServiceProviderSpecialization>
{
    public void Configure(EntityTypeBuilder<ServiceProviderSpecialization> builder)
    {
        builder.ToTable("ServiceProviderSpecializations");

        builder.HasKey(sps => new {sps.ServiceProviderId, sps.SpecializationId});
    }
}
