namespace Database.Models.Services
{
    public class EmergencyCase : Service
    {
        public int TimeForArrival { get; set; }
        public string CurrentLocation { get; set; } = default!;
        public int EmergencyCategoryId { get; set; }
        public EmergencyCategory? EmergencyCategory { get; set; }
    }
}
