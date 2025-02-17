namespace Database.Models.Services
{
    public class EmergencyCase : Service
    {
        public string Location { get; set; } = default!;
        public string Category { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
