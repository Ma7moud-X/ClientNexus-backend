using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class StartServicePaymentRequestDTO
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public int ServiceProviderId { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}
