using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class GetPaymentStatusResponseDTO
    {
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
    }
}
