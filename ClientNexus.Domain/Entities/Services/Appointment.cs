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
        public Appointment()
        {
            ServiceType = ServiceType.Appointment;
        }
    }
}