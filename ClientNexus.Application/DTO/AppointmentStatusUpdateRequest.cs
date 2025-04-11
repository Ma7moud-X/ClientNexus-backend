using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTO
{
    public class AppointmentStatusUpdateRequest
    {
        public ServiceStatus Status { get; set; }
        public string? Reason { get; set; }     //cancellations
    }
}
