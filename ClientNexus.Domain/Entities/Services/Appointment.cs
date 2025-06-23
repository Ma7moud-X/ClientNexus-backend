using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Services
{
    public class Appointment : Service
    {
        public DateTime? CheckInTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationTime { get; set; }
        public bool ReminderSent { get; set; }  //for notifications
        public DateTime? ReminderSentTime { get; set; }
        public int SlotId { get; set; }
        public required Slot Slot { get; set; }
        
        //Added for Zoom Meeting integration
        public string? ZoomJoinUrl { get; set; }
        public long? ZoomMeetingId { get; set; }
        public string? HostStartUrl { get; set; }

        // Added for Google Meet integration
        //public string? MeetingLink { get; set; }
        //public string? GoogleCalendarEventId { get; set; }  // To track calendar event for updates/deletions
        
        public Appointment()
        {
            ServiceType = ServiceType.Appointment;
        }
    }
}