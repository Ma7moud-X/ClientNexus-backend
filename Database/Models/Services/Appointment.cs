namespace Database.Models.Services
{
    public enum AppointmentType
    {
        Online = 'O',
        InPerson = 'I',
        PhoneCall = 'P',
    }

    public class Appointment : Service
    {
        public decimal Price { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateTime Date { get; set; }

        public int SlotId { get; set; }
        public Slot? Slot { get; set; }
    }
}
