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
        public int ServiceProviderId { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
