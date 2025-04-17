using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClientNexus.Domain.Entities.Others;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace ClientNexus.Infrastructure.Configurations.Others;

public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Abbreviation)
            .HasMaxLength(10);

        builder.HasMany(c => c.Addresses)
            .WithOne(a => a.City)
            .HasForeignKey(a => a.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        //data seeding
        builder.HasData(
            new City { Id = 1, Name = "مدينة نصر", StateId = 1, CountryId = 1 },
            new City { Id = 2, Name = "الهرم", StateId = 2, CountryId = 1 }
        );
    }
}
