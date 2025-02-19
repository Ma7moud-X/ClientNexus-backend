using Database.Models.Others;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Others;

public class ServicePaymentConfig : IEntityTypeConfiguration<ServicePayment>
{
    public void Configure(EntityTypeBuilder<ServicePayment> builder)
    {
        builder.ToTable("ServicePayments");

        builder
            .HasOne(sp => sp.Service)
            .WithOne(s => s.ServicePayment)
            .HasForeignKey<ServicePayment>(sp => sp.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
