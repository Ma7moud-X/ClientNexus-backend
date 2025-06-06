using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Services
{
    public class Slot
    {
        public int Id { get; set; } 
        public DateTime Date { get; set; }
        public SlotStatus Status { get; set; } = SlotStatus.Available;
        public SlotType SlotType { get; set; }

        public int ServiceProviderId { get; set; }
        public int? AvailableDayId { get; set; }
        
        public ServiceProvider? ServiceProvider { get; set; }
        public AvailableDay? AvailableDay { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }   
    }
}
