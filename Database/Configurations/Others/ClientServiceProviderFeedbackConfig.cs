using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Services
{
    public class ClientServiceProviderFeedbackConfig : IEntityTypeConfiguration<ClientServiceProviderFeedback>
    {
        public void Configure(EntityTypeBuilder<ClientServiceProviderFeedback> builder)
        {
            builder.ToTable("ClientServiceProviderFeedbacks");

            builder.HasKey(ssp => new { ssp.ClientId, ssp.ServiceProviderId});

            builder.HasOne(ssp => ssp.Client)
                .WithMany(s => s.ClientServiceProviderFeedbacks)
                .HasForeignKey(ssp => ssp.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ssp => ssp.ServiceProvider)
                .WithMany(sp => sp.ClientServiceProviderFeedbacks)
                .HasForeignKey(ssp => ssp.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}