using Database.Models;
using Database.Models.Others;
using Database.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
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
                .HasColumnType("varchar(1)")
                .HasConversion(type => type.ToChar(), type => type.ToPaymentStatus())
                .IsRequired();

            builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");

            builder
                .Property(p => p.PaymentType)
                .HasConversion(type => type.ToChar(), type => type.ToPaymentType())
                .IsRequired()
                .HasColumnType("varchar(1)");
        }
    }
}
