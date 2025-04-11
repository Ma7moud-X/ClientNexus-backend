using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class StateDTO
    {
        [Required]
        public string Name { get; set; } = default!;
        public string? Abbreviation { get; set; }
    }
}
