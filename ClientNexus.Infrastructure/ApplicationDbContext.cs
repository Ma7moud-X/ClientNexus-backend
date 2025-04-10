using ClientNexus.Infrastructure.Configurations;
using ClientNexus.Infrastructure.Configurations.Content;
using ClientNexus.Infrastructure.Configurations.Others;
using ClientNexus.Infrastructure.Configurations.Services;
using ClientNexus.Infrastructure.Configurations.Users;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ClientNexus.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<BaseUser, IdentityRole<int>, int>  // Use IdentityRole<int> here
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default schema for the database
            modelBuilder.HasDefaultSchema("ClientNexusSchema");

            // Custom entity configurations
            new BaseUserConfig().Configure(modelBuilder.Entity<BaseUser>());
            new ClientConfig().Configure(modelBuilder.Entity<Client>());
            new AdminConfig().Configure(modelBuilder.Entity<Admin>());
            new ServiceProviderConfig().Configure(modelBuilder.Entity<ServiceProvider>());
            new ServiceProviderTypeConfig().Configure(modelBuilder.Entity<ServiceProviderType>());
            new SpecializationConfig().Configure(modelBuilder.Entity<Specialization>());
            new ServiceProviderSpecializationConfig().Configure(modelBuilder.Entity<ServiceProviderSpecialization>());

            new ServiceConfig().Configure(modelBuilder.Entity<Service>());
            new EmergencyCaseConfig().Configure(modelBuilder.Entity<EmergencyCase>());
            new EmergencyCategoryConfig().Configure(modelBuilder.Entity<EmergencyCategory>());
            new QuestionConfig().Configure(modelBuilder.Entity<Question>());
            new ConsultationCaseConfiguration().Configure(modelBuilder.Entity<ConsultationCase>());
            new AppointmentConfig().Configure(modelBuilder.Entity<Appointment>());
            new AppointmentCostConfig().Configure(modelBuilder.Entity<AppointmentCost>());

            new PaymentConfig().Configure(modelBuilder.Entity<Payment>());
            new ServicePaymentConfig().Configure(modelBuilder.Entity<ServicePayment>());
            new SubscriptionPaymentConfig().Configure(modelBuilder.Entity<SubscriptionPayment>());
            new ProblemConfig().Configure(modelBuilder.Entity<Problem>());
            new CaseFileConfiguration().Configure(modelBuilder.Entity<CaseFile>());
            new OfficeImageUrlConfig().Configure(modelBuilder.Entity<OfficeImageUrl>());

            new PhoneNumberConfig().Configure(modelBuilder.Entity<PhoneNumber>());
            new AccessLevelConfig().Configure(modelBuilder.Entity<AccessLevel>());
            new AddressConfig().Configure(modelBuilder.Entity<Address>());
            new CityConfig().Configure(modelBuilder.Entity<City>());
            new StateConfig().Configure(modelBuilder.Entity<State>());
            new CountryConfig().Configure(modelBuilder.Entity<Country>());

            new SlotConfig().Configure(modelBuilder.Entity<Slot>());

            new DocumentConfig().Configure(modelBuilder.Entity<Document>());
            new CategoryConfig().Configure(modelBuilder.Entity<DCategory>());
            new DocumentCategoryConfig().Configure(modelBuilder.Entity<DocumentCategory>());
            new DocumentTypeConfig().Configure(modelBuilder.Entity<DocumentType>());

            //new LicenseConfig().Configure(modelBuilder.Entity<License>());

            new ClientServiceProviderFeedbackConfig().Configure(modelBuilder.Entity<ClientServiceProviderFeedback>());
        }

        // DbSets
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
        public DbSet<AppointmentCost> AppointmentCosts { get; set; }

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
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Country> Countries { get; set; }

        // Scheduling entities
        public DbSet<Slot> Slots { get; set; }

        // Content entities
        public DbSet<Document> Documents { get; set; }
        public DbSet<DCategory> DCategories { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<OfficeImageUrl> OfficeImageUrls { get; set; }

        // Lawyer-specific entities

        // Feedback entities
        public DbSet<ClientServiceProviderFeedback> ClientServiceProviderFeedbacks { get; set; }
    }
}
