namespace Database.Models.Services
{
    public class EmergencyCase : Service
    {
        public string Location { get; set; } = default!;
        public decimal Price { get; set; }

        public int EmergencyCategoryId { get; set; }
        public EmergencyCategory? EmergencyCategory { get; set; }
    }
}
