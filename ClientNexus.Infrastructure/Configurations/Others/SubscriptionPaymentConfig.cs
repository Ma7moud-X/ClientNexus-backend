using ClientNexus.Domain.Entities.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Infrastructure.Configurations.Others;

public class SubscriptionPaymentConfig : IEntityTypeConfiguration<SubscriptionPayment>
{
    //public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    //{
    //    builder.ToTable("SubscriptionPayments");

    //    builder
    //        .HasOne(sp => sp.ServiceProvider)
    //        .WithMany(p => p.SubscriptionPayments)
    //        .HasForeignKey(sp => sp.ServiceProviderId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    builder.Property(subpay => subpay.SubscriptionType).HasColumnType("char(1)").IsRequired().HasConversion(st => (char)st, st => (SubscriptionType)st);
    //}

    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.ToTable("SubscriptionPayments");

        builder.Property(sp => sp.SubscriptionTier)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(sp => sp.SubscriptionType)
            .HasColumnType("char(1)")
            .IsRequired();

        builder.HasOne(sp => sp.ServiceProvider)
            .WithMany(p => p.SubscriptionPayments)
            .HasForeignKey(sp => sp.ServiceProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
