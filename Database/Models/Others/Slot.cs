using Database.Models.Users;

namespace Database.Models.Services
{
    public enum SlotStatus
    {
        Available = 'A',
        Pending = 'P',
        Booked = 'B',
        Deleted = 'D',
    }

    public enum SlotType
    {
        Online = 'O',
        InPerson = 'I',
        PhoneCall = 'P',
    }

    public class Slot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public SlotStatus Status { get; set; } = SlotStatus.Available;
        public SlotType SlotType { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }   
    }
}
