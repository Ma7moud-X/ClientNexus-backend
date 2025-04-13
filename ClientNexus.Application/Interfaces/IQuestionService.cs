using ClientNexus.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionResponseDTO> CreateQuestionAsync(int clientId, [FromBody] QuestionCreateDTO dto);
        Task<QuestionResponseDTO> CreateAnswerAsync(int questionId, int providerId, [FromBody] AnswerCreateDTO dto);
        Task<List<QuestionResponseDTO>> GetQuestionsByClientAsync(int clientId, int offset, int limit);
        Task<List<QuestionResponseDTO>> GetQuestionsAnsweredByProviderAsync(int providerId, int offset, int limit);
        Task<List<QuestionResponseDTO>> GetAllQuestionsAsync(int offset, int limit, bool onlyUnanswered = false);
    }
}
