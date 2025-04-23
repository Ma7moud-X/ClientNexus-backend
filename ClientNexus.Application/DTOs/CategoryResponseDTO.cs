using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class CategoryResponseDTO
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
