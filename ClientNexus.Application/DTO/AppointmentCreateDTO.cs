using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTO
{
    public class AppointmentCreateDTO : ServiceCreateDTO
    {
        [Required]
        public AppointmentType AppointmentType { get; set; }
        [Required]
        public int AppointmentProviderId { get; set; }
        [Required]
        public int SlotId { get; set; }
    }
}
