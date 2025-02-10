namespace Database.Models.Users;

public class PhoneNumber
{
    public int Id { get; set; }
    public string Number { get; set; }

    public int BaseUserId { get; set; }
    public BaseUser? BaseUser { get; set; }
}
