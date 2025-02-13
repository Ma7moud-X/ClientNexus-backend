using Database.Models.Content;

namespace Database.Models.Users;

public class Admin : BaseUser
{
    public int? ApprovedById { get; set; }
    public Admin? ApprovingAdmin { get; set; }

    public int AccessLevelId { get; set; }
    public AccessLevel? AccessLevel { get; set; }


    public List<Admin>? ApprovedAdmins { get; set; }
    public List<ServiceProvider>? ApprovedServiceProviders { get; set; }
    public List<Document>? Documents { get; set; }
    public List<Problem>? Problems { get; set; }
    public List<BaseUser>? BlockedUsers { get; set; }
}
