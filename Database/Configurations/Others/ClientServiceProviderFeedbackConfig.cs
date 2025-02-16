using Database.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Services
{
    public class ClientServiceProviderFeedbackConfig
        : IEntityTypeConfiguration<ClientServiceProviderFeedback>
    {
        public void Configure(EntityTypeBuilder<ClientServiceProviderFeedback> builder)
        {
            builder.ToTable("ClientServiceProviderFeedbacks");

            builder.HasKey(c => new { c.ClientId, c.ServiceProviderId });
            builder.Property(cs => cs.Feedback).HasColumnType("nvarchar(1000)").IsRequired();

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
