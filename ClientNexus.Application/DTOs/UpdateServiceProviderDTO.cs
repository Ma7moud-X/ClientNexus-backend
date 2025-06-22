using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class UpdateServiceProviderDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]


        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]

        public DateOnly BirthDate { get; set; }
        [Required]

        public string PhoneNumber { get; set; }

        public IFormFile? MainImage { get; set; }
        [Required]

        public int Office_consultation_price { get; set; }
        [Required]

        public int Telephone_consultation_price { get; set; }
        [Required]

        public int YearsOfExperience { get; set; }
        [Required]

        public string Description { get; set; }

        [Required]

        public List<AddressDTO> Addresses { get; set; } = new();

    }
}
