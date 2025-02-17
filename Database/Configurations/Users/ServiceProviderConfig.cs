using Database.Models.Services;
using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users
{
    public class ServiceProviderConfig : IEntityTypeConfiguration<ServiceProvider>
    {
        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
            builder.ToTable("ServiceProviders");

            builder.Property(sp => sp.Description).IsRequired().HasMaxLength(1000);

            builder.Property(sp => sp.MainImage).IsRequired();

            builder.Property(sp => sp.Rate).HasDefaultValue(0.0f);

            builder.Property(sp => sp.ApprovedById).IsRequired(true);

            builder.Property(sp => sp.IsFeatured).HasDefaultValue(false);

            builder.Property(sp => sp.IsApproved).HasDefaultValue(false);

            builder.Property(sp => sp.IsAvailableForEmergency).HasDefaultValue(false);

            builder
                .Property(sp => sp.MapLocation)
                .HasMaxLength(500)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)");

            builder
                .HasOne(sp => sp.ApprovingAdmin)
                .WithMany(sp => sp.ApprovedServiceProviders)
                .HasForeignKey(sp => sp.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(sp => sp.SlotServiceProviders)
                .WithOne(sp => sp.ServiceProvider)
                .HasForeignKey(sp => sp.ServiceProviderId)
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
