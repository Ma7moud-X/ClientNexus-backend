using Database.Models.Users;

namespace Database.Models.Services;

public class AppointmentCost
{
    public AppointmentType AppointmentType { get; set; }
    public decimal Cost { get; set; }

    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; }
}
