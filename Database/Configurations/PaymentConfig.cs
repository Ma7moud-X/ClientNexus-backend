using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Signature)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.ReferenceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.PaymentGateway)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.ServiceType)
                .IsRequired()
                .HasConversion<string>();

            // Relationships
            builder.HasOne(p => p.Client)
               .WithMany(c => c.Payments)
               .HasForeignKey(p => p.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ServiceProvider)
               .WithMany(sp => sp.Payments)
               .HasForeignKey(p => p.ServiceProviderId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}