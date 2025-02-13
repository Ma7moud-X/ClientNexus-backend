using Database.Models.Users;

namespace Database.Models;

public class AccessLevel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Admin>? Admins { get; set; }
}
