using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        //public void Configure(EntityTypeBuilder<Payment> builder)
        //{
        //    builder.ToTable("Payments");

        //    builder.HasKey(p => p.Id);

        //    builder.Property(p => p.Signature).IsRequired().HasMaxLength(100);

        //    builder.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);

        //    builder.Property(p => p.ReferenceNumber).IsRequired().HasMaxLength(50);

        //    builder.Property(p => p.PaymentGateway).IsRequired().HasMaxLength(50);

        //    builder
        //        .Property(p => p.Status)
        //        .HasColumnType("char(1)")
        //        .HasConversion(status => (char)status, status => (PaymentStatus)status)
        //        .IsRequired();

        //    builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");

        //    builder
        //        .Property(p => p.PaymentType)
        //        .HasConversion(type => (char)type, type => (PaymentType)type)
        //        .IsRequired()
        //        .HasColumnType("char(1)");
        //}

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Signature).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(p => p.ReferenceNumber).IsRequired().HasMaxLength(50);
            builder.Property(p => p.PaymentGateway).IsRequired().HasMaxLength(50);

            // Configure PaymentStatus enum
            builder.Property(p => p.Status)
                .HasColumnType("char(1)")
                .HasConversion(
                    status => (char)status, // Convert enum to char
                    value => (PaymentStatus)value // Convert char back to enum
                )
                .IsRequired();

            builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");

            // Configure PaymentType enum
            builder.Property(p => p.PaymentType)
                .HasColumnType("char(1)")
                .HasConversion(
                    type => (char)type, // Convert enum to char
                    value => (PaymentType)value // Convert char back to enum
                )
                .IsRequired();

            // Configure Paymob-specific fields (from Step 1)
            builder.Property(p => p.IntentionId).IsRequired().HasMaxLength(100);
            builder.Property(p => p.ClientSecret).IsRequired().HasMaxLength(100);
            builder.Property(p => p.WebhookStatus).HasMaxLength(50);
        }

    }
}
