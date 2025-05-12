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
        public int? ServiceProviderId { get; set; }
    }
    public class QuestionResponsePDTO
    {
        public int Id { get; set; }
        public string QuestionBody { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;  //question status
        public string? AnswerBody { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public bool? IsAnswerHelpful { get; set; }
        public int ClientId { get; set; }
        public DateOnly ClientBirthDate { get; set; }
        public Gender ClientGender { get; set; }
        public int? ServiceProviderId { get; set; }
        public string? ServiceProviderFirstName { get; set; }
        public string? ServiceProviderLastName { get; set; }
    }
    public class QuestionResponseCDTO
    {
        public int Id { get; set; }
        public string QuestionBody { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;  //question status
        public string? AnswerBody { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public bool? IsAnswerHelpful { get; set; }
        //public int ClientId { get; set; }
        public int? ServiceProviderId { get; set; }
        public string? ServiceProviderFirstName { get; set; }
        public string? ServiceProviderLastName { get; set; }
    }
}
