using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class EmergencyCaseConfig : IEntityTypeConfiguration<EmergencyCase>
    {
        public void Configure(EntityTypeBuilder<EmergencyCase> builder)
        {
            builder.ToTable("EmergencyCases");

            builder.HasBaseType<Service>();
        }
    }
}
