using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO
{
    public class AppointmentCreateDTO
    {
        [Required]
        public int SlotId { get; set; }

    }
}
