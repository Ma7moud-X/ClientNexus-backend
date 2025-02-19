using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscriptions");

            builder.Property(s => s.Type).HasConversion(st => (char)st, st => (SubscriptionType)st).HasColumnType("varchar(1)").IsRequired(true);

            builder.Property(s => s.Status).HasConversion(st => (char)st, st => (SubscriptionStatus)st).HasColumnType("varchar(1)").IsRequired(true);

            builder.Property(s => s.ExpireDate).IsRequired(true);

            builder.Property(s => s.Price).IsRequired().HasColumnType("decimal(18,2)");


            builder
                .HasOne(s => s.ServiceProvider)
                .WithOne(sp => sp.Subscription)
                .HasForeignKey<Subscription>(sp => sp.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
