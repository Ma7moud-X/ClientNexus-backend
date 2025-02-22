using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities;

public class PhoneNumber
{
    public int BaseUserId { get; set; }
    public int Id { get; set; }
    
    public string Number { get; set; } = default!;

    public BaseUser? BaseUser { get; set; }
}
