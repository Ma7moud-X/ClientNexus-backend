using Core.Interfaces.Repositories;
using Database;
using Database.Models;
using Database.Models.Content;
using Database.Models.Roles;
using Database.Models.Services;
using Database.Models.Users;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IBaseRepo<Category> Categories { get; private set; }

    public IBaseRepo<Document> Documents { get; private set; }

    public IBaseRepo<DocumentCategory> DocumentCategories { get; private set; }

    public IBaseRepo<BaseUser> BaseUsers { get; private set; }

    public IBaseRepo<Role> Roles { get; private set; }

    public IBaseRepo<Client> Clients { get; private set; }

    public IBaseRepo<ServiceProvider> ServiceProviders { get; private set; }

    public IBaseRepo<Admin> Admins { get; private set; }

    public IBaseRepo<AccessLevel> AccessLevels { get; private set; }

    public IBaseRepo<Lawyer> Lawyers { get; private set; }

    public IBaseRepo<LawyerLicence> LawyerLicences { get; private set; }

    public IBaseRepo<LawyerSpecialization> LawyerSpecializations { get; private set; }

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

    public IBaseRepo<Subscription> Subscriptions { get; private set; }

    public IBaseRepo<Slot> Slots { get; private set; }

    public IBaseRepo<SlotType> SlotTypes { get; private set; }

    public IBaseRepo<SlotServiceProvider> SlotServiceProviders { get; private set; }

    public readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        // Document Models
        Categories = new BaseRepo<Category>(context);
        Documents = new BaseRepo<Document>(context);
        DocumentCategories = new BaseRepo<DocumentCategory>(context);

        // User Models
        BaseUsers = new BaseRepo<BaseUser>(context);
        Roles = new BaseRepo<Role>(context);
        Clients = new BaseRepo<Client>(context);
        ServiceProviders = new BaseRepo<ServiceProvider>(context);

        Admins = new BaseRepo<Admin>(context);
        AccessLevels = new BaseRepo<AccessLevel>(context);

        Lawyers = new BaseRepo<Lawyer>(context);
        LawyerLicences = new BaseRepo<LawyerLicence>(context);
        LawyerSpecializations = new BaseRepo<LawyerSpecialization>(context);
        Addresses = new BaseRepo<Address>(context);
        Feedbacks = new BaseRepo<ClientServiceProviderFeedback>(context);
        PhoneNumbers = new BaseRepo<PhoneNumber>(context);

        // Services
        Services = new BaseRepo<Service>(context);
        Questions = new BaseRepo<Question>(context);
        EmergencyCases = new BaseRepo<EmergencyCase>(context);
        ConsultationCases = new BaseRepo<ConsultationCase>(context);
        Appointments = new BaseRepo<Appointment>(context);
        CaseFiles = new BaseRepo<CaseFile>(context);

        // Others
        Payments = new BaseRepo<Payment>(context);
        Problems = new BaseRepo<Problem>(context);
        Subscriptions = new BaseRepo<Subscription>(context);
        Slots = new BaseRepo<Slot>(context);
        SlotTypes = new BaseRepo<SlotType>(context);
        SlotServiceProviders = new BaseRepo<SlotServiceProvider>(context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<IEnumerable<T>> FromSqlQueryListAsync<T>(
        string query,
        params SqlParameter[] parameters
    )
    {
        return await _context.Database.SqlQueryRaw<T>(query, parameters).ToListAsync();
    }

    public async Task<T?> FromSqlQuerySingleAsync<T>(string query, params SqlParameter[] parameters)
    {
        return await _context.Database.SqlQueryRaw<T>(query, parameters).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> ExecuteSqlAsync(string query, params SqlParameter[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(query, parameters);
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
