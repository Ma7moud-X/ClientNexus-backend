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

    }
}
