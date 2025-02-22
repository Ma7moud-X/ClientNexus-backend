using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Services;

public class AppointmentCost
{
    public AppointmentType AppointmentType { get; set; }
    public decimal Cost { get; set; }

    public int ServiceProviderId { get; set; }
    public ServiceProvider? ServiceProvider { get; set; }
}
