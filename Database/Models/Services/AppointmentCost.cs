namespace Database.Models.Services;

public class AppointmentCost
{
    public int ServiceProviderId { get; set; }
    public AppointmentType AppointmentType { get; set; }
    public decimal Cost { get; set; }
}
