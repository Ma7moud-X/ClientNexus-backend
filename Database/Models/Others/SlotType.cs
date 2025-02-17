namespace Database.Models.Services
{
    public class SlotType
    {
        public int Id { get; set; }
        public AppointmentType Type { get; set; }
        
        public int SlotId { get; set; }
        public Slot? Slot { get; set; }
    }
}