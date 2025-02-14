using Microsoft.AspNetCore.Identity;

namespace Database.Models.Users;

public class BaseUser : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsBlocked { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateOnly BirthDate { get; set; }


    public int? BlockedById { get; set; }
    public Admin? BlockedBy { get; set; }

    public ICollection<PhoneNumber>? PhoneNumbers { get; set; }
}
