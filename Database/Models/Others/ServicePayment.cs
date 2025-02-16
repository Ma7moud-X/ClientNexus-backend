using Database.Models.Services;

namespace Database.Models.Others;

public class ServicePayment: Payment
{
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
}
