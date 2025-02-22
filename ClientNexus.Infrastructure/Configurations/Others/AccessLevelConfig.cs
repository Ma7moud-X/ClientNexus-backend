using ClientNexus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClientNexus.Infrastructure.Configurations;

public class AccessLevelConfig : IEntityTypeConfiguration<AccessLevel>
{
    public void Configure(EntityTypeBuilder<AccessLevel> builder)
    {
        builder.ToTable("AccessLevels");

        builder.Property(l => l.Name)
            .HasColumnType("varchar(50)")
            .IsRequired();

        // pre-populate
        List<AccessLevel> accessLevels =
        [
            new AccessLevel { Id = -1, Name = "Super Admin" },
            new AccessLevel { Id = -2, Name = "Customer Service" },
        ];

        builder.HasData(accessLevels);
    }
}
