using ClientNexus.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO
{
    public class AppointmentDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public ServiceStatus Status { get; set; }
        public int ClientId { get; set; }
        public int ServiceProviderId { get; set; }
        [Required]
        public int SlotId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationTime { get; set; }
        public bool ReminderSent { get; set; }  //for notifications
        public DateTime? ReminderSentTime { get; set; }

        //public bool IsVideoAppointment { get; set; }
        public string? ZoomJoinUrl { get; set; }
        public string? ZoomMeetingId { get; set; }
        //public string? HostStartUrl { get; set; }
        //public string? MeetingLink { get; set; }
        //public string? GoogleCalendarEventId { get; set; }
    }
    public class AppointmentDTO2
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public ServiceStatus Status { get; set; }
        public int ClientId { get; set; }
        public int ServiceProviderId { get; set; }
        [Required]
        public int SlotId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationTime { get; set; }
        public bool ReminderSent { get; set; }  //for notifications
        public DateTime? ReminderSentTime { get; set; }
        //public string? MeetingLink { get; set; }
        //public string? GoogleCalendarEventId { get; set; }

        // Added provider info
        public string? ServiceProviderFirstName { get; set; }
        public string? ServiceProviderLastName { get; set; }
        public string? ServiceProviderMainImage { get; set; }
        public string? ServiceProviderMainSpecialization { get; set; }

        public string? ServiceProviderCity { get; set; }

        public float ServiceProviderRate { get; set; }

        // Added slot info
        public SlotType SlotType { get; set; }

        public DateTime SlotDate { get; set; }
        public string? ZoomJoinUrl { get; set; }
        public long? ZoomMeetingId { get; set; }
        //public string? HostStartUrl { get; set; }

    }
}
