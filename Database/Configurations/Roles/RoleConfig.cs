using Database.Models.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Roles;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        List<Role> roles =
        [
            new Role
            {
                Id = -1,
                Name = "Admin",
                NormalizedName = "ADMIN",
            },
            new Role
            {
                Id = -2,
                Name = "Client",
                NormalizedName = "CLIENT",
            },
            new Role
            {
                Id = -3,
                Name = "Service Provider",
                NormalizedName = "SERVICE PROVIDER",
            },
        ];

        builder.HasData(roles);
    }
}
