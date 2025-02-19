using Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Users;

public class ServiceProviderTypeConfig : IEntityTypeConfiguration<ServiceProviderType>
{
    public void Configure(EntityTypeBuilder<ServiceProviderType> builder)
    {
        builder.ToTable("ServiceProviderTypes");

        builder.Property(st => st.Name).HasColumnType("nvarchar(50)").IsRequired();
        builder
            .HasMany(t => t.ServiceProviders)
            .WithOne(s => s.Type)
            .HasForeignKey(s => s.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        List<ServiceProviderType> serviceProviderTypes =
        [
            new ServiceProviderType { Id = -1, Name = "Lawyer" },
        ];

        builder.HasData(serviceProviderTypes);
    }
}
