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

            builder.Property(s => s.Type).HasColumnType("varchar(1)").IsRequired(true);

            builder.Property(s => s.Status).HasColumnType("varchar(1)").IsRequired(true);

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
