using ClientNexus.Domain.Enums;

namespace ClientNexus.Domain.Entities.Services
{
    public class Question : Service
    {
        public required string QuestionBody { get; set; }
        public bool? Visibility { get; set; }
        public string? AnswerBody { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public bool? IsAnswerHelpful { get; set; }

        public Question()
        {
            ServiceType = ServiceType.Question;
        }
    }
}