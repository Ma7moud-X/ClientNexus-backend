using ClientNexus.Domain.Entities.Services;

namespace ClientNexus.Domain.Entities.Others;

public class ServicePayment: Payment
{
  

    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public string ServiceName { get; set; } // Name of the one-time service
}
