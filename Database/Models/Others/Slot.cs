namespace Database.Models.Services
{
    public enum SlotStatus
    {
        Available = 'A',
        Pending = 'P',
        Booked = 'B',
        Deleted = 'D',
    }

    public class Slot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public char Status { get; set; } = (char)SlotStatus.Available;

        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<SlotType>? SlotTypes { get; set; }
        public ICollection<SlotServiceProvider>? SlotServiceProviders { get; set; }
    }
}
