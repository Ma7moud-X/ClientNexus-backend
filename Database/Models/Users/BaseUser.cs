using Microsoft.AspNetCore.Identity;

namespace Database.Models.Users;

public enum UserType
{
    Client = 'C',
    ServiceProvider = 'S',
    Admin = 'A',
}

public class BaseUser : IdentityUser<int>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsBlocked { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateOnly BirthDate { get; set; }
    public UserType UserType { get; set; }

    public int? BlockedById { get; set; }
    public Admin? BlockedBy { get; set; }

    public ICollection<PhoneNumber>? PhoneNumbers { get; set; }
    public ICollection<Address>? Addresses { get; set; }
}
