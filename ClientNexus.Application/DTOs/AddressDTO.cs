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
        [Required]
        public int CityId { get; set; }
        [Required]
        public int StateId { get; set; }
        // Optional 
        public string? CityName { get; set; }
        public string? StateName { get; set; }

    }
}
