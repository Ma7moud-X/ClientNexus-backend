namespace Database.Models.Services
{
    public class SlotType
    {
        public int Id { get; set; }
        public AppointmentType Type { get; set; }
        
        // Foreign key for Slot
        public int SlotId { get; set; }
        // Navigation property
        public Slot Slot { get; set; }
    }
}