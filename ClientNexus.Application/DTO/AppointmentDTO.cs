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

        // Added provider info
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ServiceProviderMainSpecialization { get; set; }

        public string? ServiceProviderCity { get; set; }

        public float ServiceProviderRate { get; set; }

        // Added slot info
        public SlotType SlotType { get; set; }

        public DateTime SlotDate { get; set; }

    }
}
