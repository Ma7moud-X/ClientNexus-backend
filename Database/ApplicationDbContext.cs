using Database.Configurations;
using Database.Configurations.Content;
using Database.Configurations.Roles;
using Database.Configurations.Users;
using Database.Models;
using Database.Models.Content;
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

        new AdminConfig().Configure(modelBuilder.Entity<Admin>());
        new AccessLevelConfig().Configure(modelBuilder.Entity<AccessLevel>());

        new DocumentConfig().Configure(modelBuilder.Entity<Document>());
        new CategoryConfig().Configure(modelBuilder.Entity<Category>());
        
        new ServiceProviderConfig().Configure(modelBuilder.Entity<ServiceProvider>());
        
        new AddressConfig().Configure(modelBuilder.Entity<Address>());
        new SubscriptionConfig().Configure(modelBuilder.Entity<Subscription>());
    }

    // BaseUser
    public DbSet<BaseUser> BaseUsers { get; set; }
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    
    // Admin
    public DbSet<Admin> Admins { get; set; }
    public DbSet<AccessLevel> AccessLevels { get; set; }

    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    
}
