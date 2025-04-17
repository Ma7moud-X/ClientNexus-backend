using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations.Users
{
    public class ServiceProviderConfig : IEntityTypeConfiguration<ServiceProvider>
    {
        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
            builder.ToTable("ServiceProviders");

            builder.Property(sp => sp.Description).IsRequired().HasMaxLength(1000);

            builder.Property(sp => sp.MainImage).IsRequired().HasMaxLength(1000);

            builder.Property(sp => sp.Rate).HasDefaultValue(0.0f);

            builder.Property(sp => sp.ApprovedById).IsRequired(false);

            builder.Property(sp => sp.IsFeatured).HasDefaultValue(false);

            builder.Property(sp => sp.IsApproved).HasDefaultValue(false);

            builder.Property(sp => sp.IsAvailableForEmergency).HasDefaultValue(false);

            builder
                .Property(sp => sp.SubscriptionStatus)
                .HasConversion(st => (char)st, st => (SubscriptionStatus)st)
                .HasColumnType("char(1)")
                .IsRequired();

            builder
                .Property(sp => sp.SubType)
                .HasConversion(st => (char)st, st => (SubscriptionType)st)
                .HasColumnType("char(1)")
                .IsRequired();

            // builder.HasIndex(sp => sp.CurrentLocation);

            builder.Property(sp => sp.SubscriptionExpiryDate).IsRequired(false);

            builder
                .HasOne(sp => sp.ApprovingAdmin)
                .WithMany(sp => sp.ApprovedServiceProviders)
                .HasForeignKey(sp => sp.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(sp => sp.ClientsWithFeedbacks)
                .WithMany(c => c.FeedbackedServiceProviders)
                .UsingEntity<ClientServiceProviderFeedback>(
                    j =>
                        j.HasOne(cspf => cspf.Client)
                            .WithMany(c => c.ClientServiceProviderFeedbacks)
                            .HasForeignKey(cspf => cspf.ClientId)
                            .OnDelete(DeleteBehavior.Restrict),
                    j =>
                        j.HasOne(cspf => cspf.ServiceProvider)
                            .WithMany(sp => sp.ClientServiceProviderFeedbacks)
                            .HasForeignKey(cspf => cspf.ServiceProviderId)
                            .OnDelete(DeleteBehavior.Restrict)
                );
        }
    }
}
