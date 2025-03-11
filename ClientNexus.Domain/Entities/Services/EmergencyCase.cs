namespace ClientNexus.Domain.Entities.Services
{
    public class EmergencyCase : Service
    {
        public int? TimeForArrival { get; set; }
        public double MeetingLongitude { get; set; }
        public double MeetingLatitude { get; set; }

        // public int EmergencyCategoryId { get; set; }
        // public EmergencyCategory? EmergencyCategory { get; set; }
    }
}
