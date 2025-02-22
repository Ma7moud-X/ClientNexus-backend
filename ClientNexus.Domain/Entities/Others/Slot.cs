using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Services
{
    public class Slot
    {
        public int ServiceProviderId { get; set; }
        public int Id { get; set; }
        
        public DateTime Date { get; set; }
        public SlotStatus Status { get; set; } = SlotStatus.Available;
        public SlotType SlotType { get; set; }

        public ServiceProvider? ServiceProvider { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }   
    }
}
