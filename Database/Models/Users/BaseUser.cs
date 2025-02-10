using Microsoft.AspNetCore.Identity;

namespace Database.Models.Users;

public class BaseUser : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public bool IsBlocked { get; set; } = false;
    public bool IsDeleted { get; set; } = false;

    public List<PhoneNumber>? PhoneNumbers { get; set; }
}
