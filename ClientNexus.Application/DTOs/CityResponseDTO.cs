using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class CityResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Abbreviation { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
    }
}
