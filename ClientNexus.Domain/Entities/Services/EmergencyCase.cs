using ClientNexus.Domain.Enums;
using NetTopologySuite.Geometries;

namespace ClientNexus.Domain.Entities.Services
{
    public class EmergencyCase : Service
    {
        public int? TimeForArrival { get; set; }
        public Point? MeetingLocation { get; set; }
        public required string MeetingTextAddress { get; set; }

        // public int EmergencyCategoryId { get; set; }
        // public EmergencyCategory? EmergencyCategory { get; set; }

        public EmergencyCase()
        {
            ServiceType = ServiceType.Emergency;
        }
    }
}
