using Database.Models.Users;

namespace Database.Models;

public class PhoneNumber
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;

    public int BaseUserId { get; set; }
    public BaseUser? BaseUser { get; set; }
}
