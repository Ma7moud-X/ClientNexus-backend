using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class SpecializationDTO
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public int ServiceProviderTypeId { get; set; }
    }
}
