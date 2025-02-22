using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Services
{
    public class ClientServiceProviderFeedbackConfig
        : IEntityTypeConfiguration<ClientServiceProviderFeedback>
    {
        public void Configure(EntityTypeBuilder<ClientServiceProviderFeedback> builder)
        {
            builder.ToTable("ClientServiceProviderFeedbacks");

            builder.HasKey(c => new { c.ServiceProviderId, c.ClientId });
            builder.Property(cs => cs.Feedback).HasColumnType("nvarchar(1000)").IsRequired(false);

            // builder
            //     .HasOne(c => c.Client)
            //     .WithMany(c => c.ClientServiceProviderFeedbacks)
            //     .HasForeignKey(c => c.ClientId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // builder
            //     .HasOne(c => c.ServiceProvider)
            //     .WithMany(c => c.ClientServiceProviderFeedbacks)
            //     .HasForeignKey(c => c.ServiceProviderId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
