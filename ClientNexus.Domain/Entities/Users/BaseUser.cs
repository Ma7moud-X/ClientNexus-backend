using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ClientNexus.Domain.Entities.Users;

public class BaseUser : IdentityUser<int>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsBlocked { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateOnly BirthDate { get; set; }
    public UserType UserType { get; set; }
    public Gender Gender { get; set; }
    public string? NotificationToken { get; set; }
    public string? MainImage { get; set; }


    public int? BlockedById { get; set; }
    public Admin? BlockedBy { get; set; }

    public ICollection<PhoneNumber>? PhoneNumbers { get; set; }
    public ICollection<Address>? Addresses { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
}
