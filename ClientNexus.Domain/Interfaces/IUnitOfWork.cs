using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Roles;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
using Microsoft.Data.SqlClient;

namespace ClientNexus.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Document Models
    IBaseRepo<DCategory> DCategories { get; }
    IBaseRepo<Document> Documents { get; }
    IBaseRepo<DocumentCategory> DocumentCategories { get; }
    IBaseRepo<DocumentType> DocumentTypes { get; }

    // User Models
    IBaseRepo<BaseUser> BaseUsers { get; }
    IBaseRepo<Role> Roles { get; }
    IBaseRepo<Client> Clients { get; }
    IBaseRepo<ServiceProvider> ServiceProviders { get; }
    IBaseRepo<ServiceProviderType> ServiceProviderTypes { get; }
    IBaseRepo<ServiceProviderSpecialization> ServiceProviderSpecializations { get; }
    IBaseRepo<Specialization> Specializations { get; }

    IBaseRepo<Admin> Admins { get; }
    IBaseRepo<AccessLevel> AccessLevels { get; }

    IBaseRepo<License> Licenses { get; }
    IBaseRepo<Address> Addresses { get; }
    IBaseRepo<City> Cities { get; }
    IBaseRepo<State> States { get; }
    IBaseRepo<Country> Countries { get; }
    IBaseRepo<ClientServiceProviderFeedback> Feedbacks { get; }
    IBaseRepo<PhoneNumber> PhoneNumbers { get; }

    // Services
    IBaseRepo<Service> Services { get; }
    IBaseRepo<Question> Questions { get; }
    IBaseRepo<EmergencyCase> EmergencyCases { get; }
    IBaseRepo<EmergencyCategory> EmergencyCategories { get; }
    IBaseRepo<ConsultationCase> ConsultationCases { get; }
    IBaseRepo<Appointment> Appointments { get; }
    IBaseRepo<AppointmentCost> AppointmentCosts { get; }
    IBaseRepo<CaseFile> CaseFiles { get; }

    // Others
    IBaseRepo<Payment> Payments { get; }
    IBaseRepo<ServicePayment> ServicePayments { get; }
    IBaseRepo<SubscriptionPayment> SubscriptionPayments { get; }
    IBaseRepo<Problem> Problems { get; }
    IBaseRepo<Slot> Slots { get; }
    IBaseRepo<OfficeImageUrl> OfficeImageUrls { get; }

    // Functions
    Task<int> SaveChangesAsync();
    Task<T?> SqlGetSingleAsync<T>(string query, params SqlParameter[] parameters);
    Task<IEnumerable<T>> SqlGetListAsync<T>(string query, params SqlParameter[] parameters);
    Task<int> SqlExecuteAsync(string query, params SqlParameter[] parameters);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
