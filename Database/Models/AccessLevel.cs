using System;
using Database.Models.Users;

namespace Database.Models;

public class AccessLevel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Admin>? Admins { get; set; }
}
