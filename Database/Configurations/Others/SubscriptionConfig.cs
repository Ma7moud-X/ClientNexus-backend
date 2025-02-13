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

            builder.Property(s => s.Type)
                .IsRequired(true)
                .HasConversion<string>();

            builder.Property(s => s.Status)
                .IsRequired(true)
                .HasConversion<string>();

            builder.Property(s => s.ExpireDate)
                .IsRequired(true);
            
            builder.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        }
    }
}