using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientNexus.API.Controllers
{
    [Route("api/qa")]
    [ApiController]
    public class QAController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        public QAController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("{questionId:int}", Name = "GetQuestionById")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            return Ok(await _questionService.GetQuestionByIdAsync(questionId));
        }

        [HttpGet("provider/{providerId:int}")]
        public async Task<IActionResult> GetQuestionsAnsweredByProvider(int providerId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            return Ok(await _questionService.GetQuestionsAnsweredByProviderAsync(providerId, offset, limit));
        }

        [HttpGet("client")]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> GetQuestionsByClient(int offset = 0, int limit = 10, bool onlyUnanswered = false)
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out var clientId))
                throw new UnauthorizedAccessException("Client ID not found in token.");

            var result = await _questionService.GetQuestionsByClientAsync(clientId, offset, limit, onlyUnanswered);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllQuestions(int offset = 0, int limit = 10, bool onlyUnanswered = false)
        {
            var result = await _questionService.GetAllQuestionsAsync(offset, limit, onlyUnanswered);
            return Ok(result);
        }

        [HttpPost("question")]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDTO dto)
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out var clientId))
                throw new UnauthorizedAccessException("Client ID not found in token.");
            
            var result = await _questionService.CreateQuestionAsync(clientId, dto);
            return CreatedAtAction(nameof(GetQuestionById), result);
        }


        [HttpPost("answer/{questionId:int}")]
        [Authorize(Roles = "S")]
        public async Task<IActionResult> CreateAnswer(int questionId, [FromBody] AnswerCreateDTO dto)
        {
            var ProviderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(ProviderIdClaim) || !int.TryParse(ProviderIdClaim, out var providerId))
                throw new UnauthorizedAccessException("Provider ID not found in token.");

            var result = await _questionService.CreateAnswerAsync(questionId, providerId, dto);
            return CreatedAtAction(nameof(GetQuestionById), result);
        }

        [HttpDelete("{questionId}")]
        [Authorize(Roles = "C,A")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID not found in token.");
            
            await _questionService.DeleteQuestionAsync(questionId, userId, role);
            return NoContent();
        }

        [HttpPatch("{questionId}")]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] string updatedBody)
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out var clientId))
                throw new UnauthorizedAccessException("Client ID not found in token.");

            await _questionService.UpdateQuestionAsync(questionId, clientId, updatedBody);
            return NoContent();
        }

        [HttpPatch("{questionId}/mark")]
        [Authorize(Roles = "C")]
        public async Task<IActionResult> MarkQuestionHelpful(int questionId, [FromQuery] bool isHelpful)
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out var clientId))
                throw new UnauthorizedAccessException("Client ID not found in token.");

            await _questionService.MarkQuestionHelpfulAsync(questionId, clientId, isHelpful);
            return NoContent();
        }


    }
}

