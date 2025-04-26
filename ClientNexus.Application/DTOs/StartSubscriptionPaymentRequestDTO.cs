using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class StartSubscriptionPaymentRequestDTO
    {
        [Required]
        public int ServiceProviderId { get; set; }

        [Required]
        public string SubscriptionType { get; set; } // "Monthly", "Quarterly", "Yearly"

        [Required]
        public string SubscriptionTier { get; set; } // "Normal", "Advanced"

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
