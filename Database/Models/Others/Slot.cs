namespace Database.Models.Services
{
    public enum SlotStatus
    {
        Available = 'A',
        Pending = 'P',
        Booked = 'B',
    }

    public class Slot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public char Status { get; set; } = (char)SlotStatus.Available;

        public Appointment? Appointment { get; set; }
        public ICollection<SlotType>? SlotTypes { get; set; }
        public ICollection<SlotServiceProvider>? SlotServiceProviders { get; set; }
    }
}
