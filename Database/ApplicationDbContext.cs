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

        new AdminConfig().Configure(modelBuilder.Entity<Admin>());
        new ClientConfiguration().Configure(modelBuilder.Entity<Client>());
        new ServiceProviderConfig().Configure(modelBuilder.Entity<ServiceProvider>());
        new LawyerConfig().Configure(modelBuilder.Entity<Lawyer>());


        new PhoneNumberConfig().Configure(modelBuilder.Entity<PhoneNumber>());

        new AccessLevelConfig().Configure(modelBuilder.Entity<AccessLevel>());

        new DocumentConfig().Configure(modelBuilder.Entity<Document>());
        new CategoryConfig().Configure(modelBuilder.Entity<Category>());
        
        new AddressConfig().Configure(modelBuilder.Entity<Address>());
        
        new SubscriptionConfig().Configure(modelBuilder.Entity<Subscription>());

        
        new LawyerLicenceConfig().Configure(modelBuilder.Entity<LawyerLicence>());
        new LawyerSpecializationConfig().Configure(modelBuilder.Entity<LawyerSpecialization>());
        
        new ProblemConfiguration().Configure(modelBuilder.Entity<Problem>());
        
        new PaymentConfig().Configure(modelBuilder.Entity<Payment>());
    }

    public DbSet<BaseUser> BaseUsers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<Lawyer> Lawyers { get; set; }
    public DbSet<Problem> Problems { get; set; }
    
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    public DbSet<AccessLevel> AccessLevels { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<LawyerLicence> LawyerLicences { get; set; }
    public DbSet<LawyerSpecialization> LawyerSpecializations { get; set; }
    public DbSet<Payment> Payments { get; set; }

}
