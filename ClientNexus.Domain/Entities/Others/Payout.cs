using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Domain.Entities.Others
{
    public class Payout
    {
        public int Id { get; set; }

        public string Signature { get; set; }

        public decimal Amount { get; set; }

        public string ReferenceNumber { get; set; }

        public string PayoutGateway { get; set; }

        public PayoutStatus Status { get; set; }

        public string PayoutId { get; set; }

        public int ServiceProviderId { get; set; }

    }
}
