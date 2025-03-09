using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Services
{
    public class Appointment : Service
    {
        public AppointmentType AppointmentType { get; set; }
        //public DateTime Date { get; set; }

        //public int AppointmentProviderId { get; set; }
        public int SlotId { get; set; }
        public Slot? Slot { get; set; }
    }
}
