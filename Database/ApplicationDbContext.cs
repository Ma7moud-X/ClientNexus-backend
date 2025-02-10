using Database.Configurations.Roles;
using Database.Configurations.Users;
using Database.Models.Roles;
using Database.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ApplicationDbContext : IdentityDbContext<BaseUser, Role, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        new RoleConfig().Configure(modelBuilder.Entity<Role>());
        new BaseUserConfig().Configure(modelBuilder.Entity<BaseUser>());
        new PhoneNumberConfig().Configure(modelBuilder.Entity<PhoneNumber>());
    }

    DbSet<PhoneNumber> PhoneNumbers { get; set; }
}
