 using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class QuestionResponseDTO
    {
        public int Id { get; set; }
        public string QuestionBody { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;  //question status
        public string? AnswerBody { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public bool? IsAnswerHelpful { get; set; }
        public int ClientId { get; set; }
        public int ServiceProviderId { get; set; }

    }
}
