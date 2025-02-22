using ClientNexus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Signature).IsRequired().HasMaxLength(100);

            builder.Property(p => p.Amount).IsRequired().HasPrecision(18, 2);

            builder.Property(p => p.ReferenceNumber).IsRequired().HasMaxLength(50);

            builder.Property(p => p.PaymentGateway).IsRequired().HasMaxLength(50);

            builder
                .Property(p => p.Status)
                .HasColumnType("char(1)")
                .HasConversion(status => (char)status, status => (PaymentStatus)status)
                .IsRequired();

            builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");

            builder
                .Property(p => p.PaymentType)
                .HasConversion(type => (char)type, type => (PaymentType)type)
                .IsRequired()
                .HasColumnType("char(1)");
        }
    }
}
