using ClientNexus.Domain.Entities.Content;

namespace ClientNexus.Domain.Entities.Users;

public class Admin : BaseUser
{
    public int? ApprovedById { get; set; }
    public Admin? ApprovingAdmin { get; set; }

    public int AccessLevelId { get; set; }
    public AccessLevel? AccessLevel { get; set; }


    public ICollection<Admin>? ApprovedAdmins { get; set; }
    public ICollection<ServiceProvider>? ApprovedServiceProviders { get; set; }
    public ICollection<Document>? UploadedDocuments { get; set; }
    public ICollection<Problem>? AssignedProblems { get; set; }
    public ICollection<BaseUser>? BlockedUsers { get; set; }
}
