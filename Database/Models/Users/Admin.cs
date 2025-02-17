using Database.Models.Content;

namespace Database.Models.Users;

public class Admin : BaseUser
{
    public int? ApprovedById { get; set; }
    public Admin? ApprovingAdmin { get; set; }

    public int AccessLevelId { get; set; }
    public AccessLevel? AccessLevel { get; set; }


    public ICollection<Admin>? ApprovedAdmins { get; set; }
    public ICollection<ServiceProvider>? ApprovedServiceProviders { get; set; }
    public ICollection<Document>? Documents { get; set; }
    public ICollection<Problem>? Problems { get; set; }
    public ICollection<BaseUser>? BlockedUsers { get; set; }
}
