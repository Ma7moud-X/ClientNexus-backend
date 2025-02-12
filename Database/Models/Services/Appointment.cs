using System;

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
    }
}
