using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFeedback(int id)
        {
            try
            {
                var feedback = await _feedbackService.GetByIdAsync(id);
                return Ok(feedback);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("provider/{serviceProviderId:int}")]
        [AllowAnonymous] // Anyone can view provider ratings
        public async Task<IActionResult> GetProviderFeedback(int serviceProviderId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllForServiceProviderAsync(serviceProviderId);
                return Ok(feedbacks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

      
        [HttpGet("client/{clientId:int}")]
        [Authorize(Roles = "Admin,Client")] // Only admin or the client themselves
        public async Task<IActionResult> GetClientFeedback(int clientId)
        {
            try
            {
                // Check if user is requesting their own data or is an admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRoleClaim != "Admin" && userIdClaim != clientId.ToString())
                {
                    return StatusCode(403, "You can only view your own feedback");
                }

                var feedbacks = await _feedbackService.GetAllByClientAsync(clientId);
                return Ok(feedbacks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("rating/{serviceProviderId:int}")]
        [AllowAnonymous] // Anyone can view provider ratings
        public async Task<IActionResult> GetProviderRating(int serviceProviderId)
        {
            try
            {
                var rating = await _feedbackService.GetAverageRatingForServiceProviderAsync(serviceProviderId);
                return Ok(rating);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO feedbackDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verify the client ID matches the authenticated user
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != feedbackDto.ClientId)
                {
                    return BadRequest($"You can only submit feedback as yourself");
                }

                var createdFeedback = await _feedbackService.CreateClientToProviderFeedbackAsync(feedbackDto);
                return CreatedAtAction(nameof(GetFeedback), new { id = createdFeedback.Id }, createdFeedback);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
     
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackDTO updateFeedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verify the user owns this feedback
                var feedback = await _feedbackService.GetByIdAsync(id);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (userIdClaim == null || int.Parse(userIdClaim) != feedback.ClientId)
                {
                    return StatusCode(403, "You can only update your own feedback");
                }

                var updatedFeedback = await _feedbackService.UpdateFeedbackAsync(updateFeedbackDto, id);
                return Ok(updatedFeedback);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                // Verify the user owns this feedback or is an admin
                var feedback = await _feedbackService.GetByIdAsync(id);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRoleClaim != "Admin" && (userIdClaim == null || int.Parse(userIdClaim) != feedback.ClientId))
                {
                    return StatusCode(403, "You can only delete your own feedback");
                }

                await _feedbackService.DeleteFeedbackAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}