using Database.Configurations;
using Database.Configurations.Content;
using Database.Configurations.Others;
using Database.Configurations.Roles;
using Database.Configurations.Services;
using Database.Configurations.Users;
using Database.Models;
using Database.Models.Content;
using Database.Models.Others;
using Database.Models.Roles;
using Database.Models.Services;
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
        new ClientConfig().Configure(modelBuilder.Entity<Client>());
        new AdminConfig().Configure(modelBuilder.Entity<Admin>());
        new ServiceProviderConfig().Configure(modelBuilder.Entity<ServiceProvider>());
        new ServiceProviderTypeConfig().Configure(modelBuilder.Entity<ServiceProviderType>());
        new SpecializationConfig().Configure(modelBuilder.Entity<Specialization>());
        new ServiceProviderSpecializationConfig().Configure(
            modelBuilder.Entity<ServiceProviderSpecialization>()
        );

        new ServiceConfig().Configure(modelBuilder.Entity<Service>());
        new EmergencyCaseConfig().Configure(modelBuilder.Entity<EmergencyCase>());
        new EmergencyCategoryConfig().Configure(modelBuilder.Entity<EmergencyCategory>());
        new QuestionConfig().Configure(modelBuilder.Entity<Question>());
        new ConsultationCaseConfiguration().Configure(modelBuilder.Entity<ConsultationCase>());
        new AppointmentConfig().Configure(modelBuilder.Entity<Appointment>());

        new PaymentConfig().Configure(modelBuilder.Entity<Payment>());
        new ServicePaymentConfig().Configure(modelBuilder.Entity<ServicePayment>());
        new SubscriptionPaymentConfig().Configure(modelBuilder.Entity<SubscriptionPayment>());
        new ProblemConfig().Configure(modelBuilder.Entity<Problem>());
        new CaseFileConfiguration().Configure(modelBuilder.Entity<CaseFile>());
        new OfficeImageUrlConfig().Configure(modelBuilder.Entity<OfficeImageUrl>());

        new PhoneNumberConfig().Configure(modelBuilder.Entity<PhoneNumber>());
        new AccessLevelConfig().Configure(modelBuilder.Entity<AccessLevel>());
        new AddressConfig().Configure(modelBuilder.Entity<Address>());
        new SubscriptionConfig().Configure(modelBuilder.Entity<Subscription>());

        new SlotConfig().Configure(modelBuilder.Entity<Slot>());

        new DocumentConfig().Configure(modelBuilder.Entity<Document>());
        new CategoryConfig().Configure(modelBuilder.Entity<DCategory>());
        new DocumentCategoryConfig().Configure(modelBuilder.Entity<DocumentCategory>());
        new DocumentTypeConfig().Configure(modelBuilder.Entity<DocumentType>());

        new LicenseConfig().Configure(modelBuilder.Entity<License>());

        new ClientServiceProviderFeedbackConfig().Configure(
            modelBuilder.Entity<ClientServiceProviderFeedback>()
        );
    }

    // User-related entities
    public DbSet<BaseUser> BaseUsers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<ServiceProviderSpecialization> ServiceProviderSpecializations { get; set; }
    public DbSet<ServiceProviderType> ServiceProviderTypes { get; set; }
    public DbSet<Specialization> Specializations { get; set; }


    // Service-related entities
    public DbSet<Service> Services { get; set; }
    public DbSet<EmergencyCase> EmergencyCases { get; set; }
    public DbSet<EmergencyCategory> EmergencyCategories { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<ConsultationCase> ConsultationCases { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    // Support entities
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ServicePayment> ServicePayments { get; set; }
    public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<CaseFile> CaseFiles { get; set; }

    // Configuration entities
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    public DbSet<AccessLevel> AccessLevels { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    // Scheduling entities
    public DbSet<Slot> Slots { get; set; }

    // Content entities
    public DbSet<Document> Documents { get; set; }
    public DbSet<DCategory> DCategories { get; set; }
    public DbSet<DocumentCategory> DocumentCategories { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<OfficeImageUrl> OfficeImageUrls { get; set; }

    // Lawyer-specific entities
    public DbSet<License> LawyerLicences { get; set; }

    // Feedback entities
    public DbSet<ClientServiceProviderFeedback> ClientServiceProviderFeedbacks { get; set; }
}
