using System;
using Database.Models.Users;

namespace Database.Models.Services
{
     public enum AppointmentType
    {
        Online,
        InPerson,
        PhoneCall
    }
    public class Appointment : Service
    {
        public bool Status { get; set; }
        public AppointmentType Type { get; set; }
        public DateTime Date { get; set; }


        // Foreign key for Slot
        public int SlotId { get; set; }
        // Navigation property for Slot
        public Slot Slot { get; set; }

        // Foreign key
        public int ServiceProviderId { get; set; }
        // Navigation property
        public ServiceProvider ServiceProvider { get; set; }
    }
}
