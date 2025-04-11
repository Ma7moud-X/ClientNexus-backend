using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class AddressDTO
    {

        [Required]
        public string DetailedAddress { get; set; }
        public string? Neighborhood { get; set; }
        public string? MapUrl { get; set; }
        [Required]
        public int CityId { get; set; }

    }
}
