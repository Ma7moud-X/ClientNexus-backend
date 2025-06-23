using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Roles;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
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
    public IBaseRepo<AvailableDay> AvailableDays { get; private set; } 

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
    public IBaseRepo<ClientServiceProviderFeedback> ClientServiceProviderFeedbacks
    {
        get;
        private set;
    }
    public IBaseRepo<EmergencyCategory> EmergencyCategories { get; private set; }

    public IBaseRepo<AppointmentCost> AppointmentCosts { get; private set; }

    public IBaseRepo<City> Cities { get; private set; }

    public IBaseRepo<State> States { get; private set; }

    public IBaseRepo<Country> Countries { get; private set; }

    public IBaseRepo<Notification> Notifications { get; private set; }
    public IBaseRepo<Payout> Payouts { get; private set; }


    public readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        // Document Entities
        DCategories = DbTryCatchDecorator<IBaseRepo<DCategory>>.Create(
            new BaseRepo<DCategory>(context)
        );
        Documents = DbTryCatchDecorator<IBaseRepo<Document>>.Create(
            new BaseRepo<Document>(context)
        );
        DocumentCategories = DbTryCatchDecorator<IBaseRepo<DocumentCategory>>.Create(
            new BaseRepo<DocumentCategory>(context)
        );

        // User Entities
        BaseUsers = DbTryCatchDecorator<IBaseRepo<BaseUser>>.Create(
            new BaseRepo<BaseUser>(context)
        );
        Roles = DbTryCatchDecorator<IBaseRepo<Role>>.Create(new BaseRepo<Role>(context));
        Clients = DbTryCatchDecorator<IBaseRepo<Client>>.Create(new BaseRepo<Client>(context));
        ServiceProviders = DbTryCatchDecorator<IBaseRepo<ServiceProvider>>.Create(
            new BaseRepo<ServiceProvider>(context)
        );

        Admins = DbTryCatchDecorator<IBaseRepo<Admin>>.Create(new BaseRepo<Admin>(context));
        AccessLevels = DbTryCatchDecorator<IBaseRepo<AccessLevel>>.Create(
            new BaseRepo<AccessLevel>(context)
        );

        //Licenses = new BaseRepo<License>(context);
        Addresses = DbTryCatchDecorator<IBaseRepo<Address>>.Create(new BaseRepo<Address>(context));
        Cities = DbTryCatchDecorator<IBaseRepo<City>>.Create(new BaseRepo<City>(context));
        States = DbTryCatchDecorator<IBaseRepo<State>>.Create(new BaseRepo<State>(context));
        Countries = DbTryCatchDecorator<IBaseRepo<Country>>.Create(new BaseRepo<Country>(context));
        Feedbacks = DbTryCatchDecorator<IBaseRepo<ClientServiceProviderFeedback>>.Create(
            new BaseRepo<ClientServiceProviderFeedback>(context)
        );
        PhoneNumbers = DbTryCatchDecorator<IBaseRepo<PhoneNumber>>.Create(
            new BaseRepo<PhoneNumber>(context)
        );

        // Services
        Services = DbTryCatchDecorator<IBaseRepo<Service>>.Create(new BaseRepo<Service>(context));
        Questions = DbTryCatchDecorator<IBaseRepo<Question>>.Create(
            new BaseRepo<Question>(context)
        );
        EmergencyCases = DbTryCatchDecorator<IBaseRepo<EmergencyCase>>.Create(
            new BaseRepo<EmergencyCase>(context)
        );
        EmergencyCategories = DbTryCatchDecorator<IBaseRepo<EmergencyCategory>>.Create(
            new BaseRepo<EmergencyCategory>(context)
        );
        ConsultationCases = DbTryCatchDecorator<IBaseRepo<ConsultationCase>>.Create(
            new BaseRepo<ConsultationCase>(context)
        );
        Appointments = DbTryCatchDecorator<IBaseRepo<Appointment>>.Create(
            new BaseRepo<Appointment>(context)
        );
        AppointmentCosts = DbTryCatchDecorator<IBaseRepo<AppointmentCost>>.Create(
            new BaseRepo<AppointmentCost>(context)
        );
        CaseFiles = DbTryCatchDecorator<IBaseRepo<CaseFile>>.Create(
            new BaseRepo<CaseFile>(context)
        );

        // Others
        Payments = DbTryCatchDecorator<IBaseRepo<Payment>>.Create(new BaseRepo<Payment>(context));
        Problems = DbTryCatchDecorator<IBaseRepo<Problem>>.Create(new BaseRepo<Problem>(context));
        Slots = DbTryCatchDecorator<IBaseRepo<Slot>>.Create(new BaseRepo<Slot>(context));
        AvailableDays = DbTryCatchDecorator<IBaseRepo<AvailableDay>>.Create(new BaseRepo<AvailableDay>(context));

        OfficeImageUrls = DbTryCatchDecorator<IBaseRepo<OfficeImageUrl>>.Create(
            new BaseRepo<OfficeImageUrl>(context)
        );

        DocumentTypes = DbTryCatchDecorator<IBaseRepo<DocumentType>>.Create(
            new BaseRepo<DocumentType>(context)
        );
        ServiceProviderTypes = DbTryCatchDecorator<IBaseRepo<ServiceProviderType>>.Create(
            new BaseRepo<ServiceProviderType>(context)
        );
        ServiceProviderSpecializations = DbTryCatchDecorator<
            IBaseRepo<ServiceProviderSpecialization>
        >.Create(new BaseRepo<ServiceProviderSpecialization>(context));
        Specializations = DbTryCatchDecorator<IBaseRepo<Specialization>>.Create(
            new BaseRepo<Specialization>(context)
        );

        ServicePayments = DbTryCatchDecorator<IBaseRepo<ServicePayment>>.Create(
            new BaseRepo<ServicePayment>(context)
        );
        SubscriptionPayments = DbTryCatchDecorator<IBaseRepo<SubscriptionPayment>>.Create(
            new BaseRepo<SubscriptionPayment>(context)
        );
        ClientServiceProviderFeedbacks = DbTryCatchDecorator<
            IBaseRepo<ClientServiceProviderFeedback>
        >.Create(new BaseRepo<ClientServiceProviderFeedback>(context));
        Notifications = DbTryCatchDecorator<IBaseRepo<Notification>>.Create(
            new BaseRepo<Notification>(context)
        );

        Payouts = DbTryCatchDecorator<IBaseRepo<Payout>>.Create(new BaseRepo<Payout>(context));
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<IEnumerable<T>> SqlGetListAsync<T>(
        string query,
        params Parameter[] parameters
    )
        where T : class
    {
        return await _context
            .Database.SqlQueryRaw<T>(
                query,
                [.. parameters.Select(p => new SqlParameter(p.Key, p.Value))]
            )
            .ToListAsync();
    }

    public async Task<T?> SqlGetSingleAsync<T>(string query, params Parameter[] parameters)
        where T : class
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
