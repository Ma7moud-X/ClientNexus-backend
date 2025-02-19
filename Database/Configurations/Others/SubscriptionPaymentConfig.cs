using Database.Models.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Others;

public class SubscriptionPaymentConfig : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.ToTable("SubscriptionPayments");

        builder
            .HasOne(sp => sp.ServiceProvider)
            .WithMany(p => p.SubscriptionPayments)
            .HasForeignKey(sp => sp.ServiceProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
