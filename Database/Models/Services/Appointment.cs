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
        public decimal Price { get; set; }
        public AppointmentType Type { get; set; }
        public DateTime Date { get; set; }


        public int SlotId { get; set; }
        public Slot Slot { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}
