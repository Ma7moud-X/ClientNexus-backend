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

            // Admin approval relationship
            builder
                .HasOne(sp => sp.ApprovingAdmin)
                .WithMany()
                .HasForeignKey(sp => sp.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Address relationship
            builder
                .HasOne(sp => sp.Address)
                .WithOne()
                .HasForeignKey<ServiceProvider>(sp => sp.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subscription relationship
            builder
                .HasOne(sp => sp.Subscription)
                .WithOne()
                .HasForeignKey<ServiceProvider>(sp => sp.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Problems relationship
            builder
                .HasMany(sp => sp.Problems)
                .WithOne(sp => sp.ServiceProvider)
                .HasForeignKey(sp => sp.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(sp => sp.Payments)
               .WithOne(p => p.ServiceProvider)
               .HasForeignKey(p => p.ServiceProviderId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(sp => sp.EmergencyCases)
                .WithOne(ec => ec.ServiceProvider)
                .HasForeignKey(ec => ec.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(sp => sp.questions)
                .WithOne(ec => ec.ServiceProvider)
                .HasForeignKey(ec => ec.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Properties configuration
            builder.Property(sp => sp.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(sp => sp.MainImage)
                .IsRequired();

            builder.Property(sp => sp.Rate)
                .HasDefaultValue(0.0f);

            builder.Property(sp => sp.ApprovedById)
                .IsRequired(true);

            builder.Property(sp => sp.IsFeatured)
                .HasDefaultValue(false);

            builder.Property(sp => sp.IsApproved)
                .HasDefaultValue(false);

            builder.Property(sp => sp.IsAvailableForEmergency)
                .HasDefaultValue(false);
        }
    }
}