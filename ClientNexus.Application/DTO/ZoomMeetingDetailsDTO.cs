using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTO
{
    public class ZoomMeetingDetailsDTO
    {
        public long? MeetingId { get; set; }
        public string? JoinUrl { get; set; }
        public string? HostStartUrl { get; set; }
    }
}
