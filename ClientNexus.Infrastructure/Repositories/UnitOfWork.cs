using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Roles;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IBaseRepo<DCategory> DCategories { get; private set; }

    public IBaseRepo<Document> Documents { get; private set; }

    public IBaseRepo<DocumentCategory> DocumentCategories { get; private set; }

    public IBaseRepo<BaseUser> BaseUsers { get; private set; }

    public IBaseRepo<Role> Roles { get; private set; }

    public IBaseRepo<Client> Clients { get; private set; }

    public IBaseRepo<ServiceProvider> ServiceProviders { get; private set; }

    public IBaseRepo<Admin> Admins { get; private set; }

    public IBaseRepo<AccessLevel> AccessLevels { get; private set; }

    //public IBaseRepo<License> Licenses { get; private set; }

    public IBaseRepo<Address> Addresses { get; private set; }

    public IBaseRepo<ClientServiceProviderFeedback> Feedbacks { get; private set; }

    public IBaseRepo<PhoneNumber> PhoneNumbers { get; private set; }

    // Services
    public IBaseRepo<Service> Services { get; private set; }

    public IBaseRepo<Question> Questions { get; private set; }

    public IBaseRepo<EmergencyCase> EmergencyCases { get; private set; }

    public IBaseRepo<ConsultationCase> ConsultationCases { get; private set; }

    public IBaseRepo<Appointment> Appointments { get; private set; }

    public IBaseRepo<CaseFile> CaseFiles { get; private set; }

    public IBaseRepo<Payment> Payments { get; private set; }

    public IBaseRepo<Problem> Problems { get; private set; }

    public IBaseRepo<Slot> Slots { get; private set; }

    public IBaseRepo<DocumentType> DocumentTypes { get; private set; }

    public IBaseRepo<ServiceProviderType> ServiceProviderTypes { get; private set; }

    public IBaseRepo<ServiceProviderSpecialization> ServiceProviderSpecializations
    {
        get;
        private set;
    }

    public IBaseRepo<Specialization> Specializations { get; private set; }

    public IBaseRepo<ServicePayment> ServicePayments { get; private set; }

    public IBaseRepo<SubscriptionPayment> SubscriptionPayments { get; private set; }

    public IBaseRepo<OfficeImageUrl> OfficeImageUrls { get; private set; }

    public IBaseRepo<EmergencyCategory> EmergencyCategories { get; private set; }

    public IBaseRepo<AppointmentCost> AppointmentCosts { get; private set; }

    public IBaseRepo<City> Cities { get; private set; }

    public IBaseRepo<State> States { get; private set; }

    public IBaseRepo<Country> Countries { get; private set; }

    public readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        // Document Entities
        DCategories = new BaseRepo<DCategory>(context);
        Documents = new BaseRepo<Document>(context);
        DocumentCategories = new BaseRepo<DocumentCategory>(context);

        // User Entities
        BaseUsers = new BaseRepo<BaseUser>(context);
        Roles = new BaseRepo<Role>(context);
        Clients = new BaseRepo<Client>(context);
        ServiceProviders = new BaseRepo<ServiceProvider>(context);

        Admins = new BaseRepo<Admin>(context);
        AccessLevels = new BaseRepo<AccessLevel>(context);

        //Licenses = new BaseRepo<License>(context);
        Addresses = new BaseRepo<Address>(context);
        Cities = new BaseRepo<City>(context);
        States = new BaseRepo<State>(context);
        Countries = new BaseRepo<Country>(context);
        Feedbacks = new BaseRepo<ClientServiceProviderFeedback>(context);
        PhoneNumbers = new BaseRepo<PhoneNumber>(context);

        // Services
        Services = new BaseRepo<Service>(context);
        Questions = new BaseRepo<Question>(context);
        EmergencyCases = new BaseRepo<EmergencyCase>(context);
        EmergencyCategories = new BaseRepo<EmergencyCategory>(context);
        ConsultationCases = new BaseRepo<ConsultationCase>(context);
        Appointments = new BaseRepo<Appointment>(context);
        AppointmentCosts = new BaseRepo<AppointmentCost>(context);
        CaseFiles = new BaseRepo<CaseFile>(context);

        // Others
        Payments = new BaseRepo<Payment>(context);
        Problems = new BaseRepo<Problem>(context);
        Slots = new BaseRepo<Slot>(context);
        OfficeImageUrls = new BaseRepo<OfficeImageUrl>(context);

        DocumentTypes = new BaseRepo<DocumentType>(context);
        ServiceProviderTypes = new BaseRepo<ServiceProviderType>(context);
        ServiceProviderSpecializations = new BaseRepo<ServiceProviderSpecialization>(context);
        Specializations = new BaseRepo<Specialization>(context);

        ServicePayments = new BaseRepo<ServicePayment>(context);
        SubscriptionPayments = new BaseRepo<SubscriptionPayment>(context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<IEnumerable<T>> SqlGetListAsync<T>(
        string query,
        params Parameter[] parameters
    )
    {
        return await _context
            .Database.SqlQueryRaw<T>(
                query,
                [.. parameters.Select(p => new SqlParameter(p.Key, p.Value))]
            )
            .ToListAsync();
    }

    public async Task<T?> SqlGetSingleAsync<T>(string query, params Parameter[] parameters)
    {
        return await _context
            .Database.SqlQueryRaw<T>(
                query,
                [.. parameters.Select(p => new SqlParameter(p.Key, p.Value))]
            )
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<int> SqlExecuteAsync(string query, params Parameter[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(
            query,
            [.. parameters.Select(p => new SqlParameter(p.Key, p.Value))]
        );
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}
