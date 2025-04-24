using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class PaymentResponseDTO
    {
        public string ClientSecret { get; set; }
        public string IntentionId { get; set; }
        public string PublicKey { get; set; }
        public string Status { get; set; }
    }
}
