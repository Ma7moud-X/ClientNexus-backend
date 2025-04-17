using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO
{
    public record SetNotificationDTO
    {
        [Required]
        public required string token { get; init; }
    }
}
