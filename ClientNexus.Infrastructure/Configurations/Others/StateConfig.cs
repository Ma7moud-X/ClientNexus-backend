using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClientNexus.Domain.Entities.Others;

namespace ClientNexus.Infrastructure.Configurations.Others;

public class StateConfig : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("States");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Abbreviation)
            .HasMaxLength(10);

        builder.HasMany(s => s.Cities)
            .WithOne(c => c.State)
            .HasForeignKey(c => c.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        //data seeding
        builder.HasData(
            new State { Id = 1, Name = "القاهرة", Abbreviation = "CA", CountryId = 1},
            new State { Id = 2, Name = "الجيزة", Abbreviation = "GZ", CountryId = 1 },
            new State { Id = 3, Name = "الاسكندرية", Abbreviation = "ALX", CountryId = 1 }
        );
    }
}
