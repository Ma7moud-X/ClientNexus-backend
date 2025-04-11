using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class CityDTO
    {
        [Required]
        public string Name { get; set; }
        public string? Abbreviation { get; set; }
        [Required]
        public int StateId { get; set; }
    }
}
