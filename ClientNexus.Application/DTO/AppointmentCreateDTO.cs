using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO
{
    public class AppointmentCreateDTO : ServiceCreateDTO
    {
        [Required]
        public int SlotId { get; set; }

    }
}
