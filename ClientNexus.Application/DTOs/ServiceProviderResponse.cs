using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class ServiceProviderResponse
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public float Rate { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string MainImage { get; set; }
        [Required]
        public int YearsOfExperience { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public List<string> SpecializationName { get; set; }
    }
}
