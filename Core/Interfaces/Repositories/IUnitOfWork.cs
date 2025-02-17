using Database.Models;
using Database.Models.Content;
using Database.Models.Others;
using Database.Models.Roles;
using Database.Models.Services;
using Database.Models.Users;
using Microsoft.Data.SqlClient;

namespace Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    // Document Models
    IBaseRepo<Category> Categories { get; }
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
    IBaseRepo<ClientServiceProviderFeedback> Feedbacks { get; }
    IBaseRepo<PhoneNumber> PhoneNumbers { get; }

    // Services
    IBaseRepo<Service> Services { get; }
    IBaseRepo<Question> Questions { get; }
    IBaseRepo<EmergencyCase> EmergencyCases { get; }
    IBaseRepo<ConsultationCase> ConsultationCases { get; }
    IBaseRepo<Appointment> Appointments { get; }
    IBaseRepo<CaseFile> CaseFiles { get; }

    // Others
    IBaseRepo<Payment> Payments { get; }
    IBaseRepo<ServicePayment> ServicePayments { get; }
    IBaseRepo<SubscriptionPayment> SubscriptionPayments { get; }
    IBaseRepo<Problem> Problems { get; }
    IBaseRepo<Subscription> Subscriptions { get; }
    IBaseRepo<Slot> Slots { get; }
    IBaseRepo<SlotType> SlotTypes { get; }
    IBaseRepo<SlotServiceProvider> SlotServiceProviders { get; }

    // Functions
    Task SaveChangesAsync();
    Task<T?> FromSqlQuerySingleAsync<T>(string query, params SqlParameter[] parameters);
    Task<IEnumerable<T>> FromSqlQueryListAsync<T>(string query, params SqlParameter[] parameters);
    Task<int> ExecuteSqlAsync(string query, params SqlParameter[] parameters);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
