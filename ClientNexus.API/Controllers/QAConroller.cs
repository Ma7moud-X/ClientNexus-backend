using ClientNexus.API.Utilities;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            return Ok(await _questionService.GetQuestionByIdAsync(questionId));
        }

        [HttpGet("provider/{providerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuestionsAnsweredByProvider(int providerId, [FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            return Ok(await _questionService.GetQuestionsAnsweredByProviderAsync(providerId, offset, limit));
        }

        [HttpGet("client")]
        [Authorize(Policy = "IsClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetQuestionsByClient(int offset = 0, int limit = 10, bool onlyUnanswered = false)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            var result = await _questionService.GetQuestionsByClientAsync(userId.Value, offset, limit, onlyUnanswered);
            return Ok(result);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllQuestions(int offset = 0, int limit = 10, bool onlyUnanswered = false)
        {
            var result = await _questionService.GetAllQuestionsAsync(offset, limit, onlyUnanswered);
            return Ok(result);
        }

        [HttpPost("question")]
        [Authorize(Policy = "IsClient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDTO dto)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            var result = await _questionService.CreateQuestionAsync(userId.Value, dto);
            return CreatedAtRoute(nameof(GetQuestionById), new { questionId = result.Id }, result);
        }


        [HttpPost("answer/{questionId:int}")]
        [Authorize(Policy = "IsServiceProvider")]
        public async Task<IActionResult> CreateAnswer(int questionId, [FromBody] AnswerCreateDTO dto)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            var result = await _questionService.CreateAnswerAsync(questionId, userId.Value, dto);
            return CreatedAtRoute(nameof(GetQuestionById), new { questionId = result.Id }, result);
        }

        [HttpDelete("{questionId}")]
        [Authorize(Policy = "IsClientOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var role = User.GetRole();
            var userId = User.GetId();
            if (role is null || userId is null)
                return Unauthorized();

            await _questionService.DeleteQuestionAsync(questionId, userId.Value, role.Value);
            return NoContent();
        }

        [HttpPatch("{questionId}")]
        [Authorize(Policy = "IsClient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] string updatedBody)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            await _questionService.UpdateQuestionAsync(questionId, userId.Value, updatedBody);
            return NoContent();
        }

        [HttpPatch("{questionId}/mark")]
        [Authorize(Policy = "IsClient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkQuestionHelpful(int questionId, [FromQuery] bool isHelpful)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            await _questionService.MarkQuestionHelpfulAsync(questionId, userId.Value, isHelpful);
            return NoContent();
        }


    }
}

