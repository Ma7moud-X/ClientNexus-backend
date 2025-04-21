using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProblem(int id)
        {
            try
            {
                var problem = await _problemService.GetProblemByIdAsync(id);
                return Ok(problem);
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
      
        [HttpGet("admin/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProblemAdminDetails(int id)
        {
            try
            {
                var problem = await _problemService.GetProblemAdminDetailsAsync(id);
                return Ok(problem);
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
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetClientProblems(int clientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim != clientId.ToString())
                {
                    return StatusCode(403, "You can only view your own problems");
                }

                // Enforce maximum page size
                if (pageSize > 50) pageSize = 50;
                
                var problems = await _problemService.GetClientProblemsAsync(clientId, pageNumber, pageSize);
                
                // Add pagination headers
                var totalCount = _problemService.GetTotalProblemCount(clientId: clientId);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                
                Response.Headers.Append("X-Pagination", 
                    System.Text.Json.JsonSerializer.Serialize(new {
                        CurrentPage = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasNext = pageNumber < totalPages,
                        HasPrevious = pageNumber > 1
                    }));
                    
                return Ok(problems);
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
        [Authorize(Roles = "ServiceProvider")]
        public async Task<IActionResult> GetServiceProviderProblems(int serviceProviderId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Check if user is requesting their own data or is an admin
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim != serviceProviderId.ToString())
                {
                    return StatusCode(403, "You can only view your own problems");
                }

                // Enforce maximum page size
                if (pageSize > 50) pageSize = 50;
                
                var problems = await _problemService.GetServiceProviderProblemsAsync(serviceProviderId, pageNumber, pageSize);
                
                // Add pagination headers
                var totalCount = _problemService.GetTotalProblemCount(serviceProviderId: serviceProviderId);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                
                Response.Headers.Append("X-Pagination", 
                    System.Text.Json.JsonSerializer.Serialize(new {
                        CurrentPage = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasNext = pageNumber < totalPages,
                        HasPrevious = pageNumber > 1
                    }));

                return Ok(problems);
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

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProblems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Enforce maximum page size
                if (pageSize > 50) pageSize = 50;
                
                var problems = await _problemService.GetAllProblemsAsync(pageNumber, pageSize);
                
                // Add pagination headers
                var totalCount = _problemService.GetTotalProblemCount();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                
                Response.Headers.Append("X-Pagination", 
                    System.Text.Json.JsonSerializer.Serialize(new {
                        CurrentPage = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasNext = pageNumber < totalPages,
                        HasPrevious = pageNumber > 1
                    }));
                    
                return Ok(problems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Client,ServiceProvider")]
        public async Task<IActionResult> CreateProblem([FromBody] CreateProblemDto createProblemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verify the user ID matches the authenticated user
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userIdClaim == null)
                {
                    return BadRequest("User ID not found in claims");
                }

                int userId = int.Parse(userIdClaim);
                if ((userRoleClaim == "Client" && userId != createProblemDto.ClientId) ||
                    (userRoleClaim == "ServiceProvider" && userId != createProblemDto.ServiceProviderId))
                {
                    return BadRequest("You can only submit problems as yourself");
                }

                var createdProblem = await _problemService.CreateProblemAsync(createProblemDto);
                return CreatedAtAction(nameof(GetProblem), new { id = createdProblem.Id }, createdProblem);
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
        [Authorize(Roles = "Client,ServiceProvider")]
        public async Task<IActionResult> UpdateProblem(int id, [FromBody] UpdateProblemDto updateProblemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the problem first to check permissions
                var problem = await _problemService.GetProblemAdminDetailsAsync(id);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userIdClaim == null)
                {
                    return BadRequest("User ID not found in claims");
                }

                int userId = int.Parse(userIdClaim);
                if ((userRoleClaim == "Client" && userId != problem.ClientId) ||
                    (userRoleClaim == "ServiceProvider" && userId != problem.ServiceProviderId))
                {
                    return StatusCode(403, "You can only update your own problems");
                }

                var updatedProblem = await _problemService.UpdateProblemAsync(updateProblemDto, id);
                return Ok(updatedProblem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Client,ServiceProvider")]
        public async Task<IActionResult> DeleteProblem(int id)
        {
            try
            {
                // Get the problem first to check permissions
                var problem = await _problemService.GetProblemAdminDetailsAsync(id);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userIdClaim == null)
                {
                    return BadRequest("User ID not found in claims");
                }

                int userId = int.Parse(userIdClaim);
                if ((userRoleClaim == "Client" && userId != problem.ClientId) ||
                    (userRoleClaim == "ServiceProvider" && userId != problem.ServiceProviderId))
                {
                    return StatusCode(403, "You can only delete your own problems");
                }

                var result = await _problemService.DeleteProblemAsync(id);
                if (result)
                    return NoContent();
                else
                    return BadRequest("Problem cannot be deleted as it's already being handled");
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

        [HttpPut("admin/{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProblemStatus(int id, [FromBody] UpdateProblemStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return BadRequest("Admin ID not found in claims");
                }

                int adminId = int.Parse(userIdClaim);
                var updatedProblem = await _problemService.UpdateProblemStatusAsync(id, statusDto, adminId);
                return Ok(updatedProblem);
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

        [HttpPut("admin/{id:int}/comment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdminComment(int id, [FromBody] AdminCommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return BadRequest("Admin ID not found in claims");
                }

                commentDto.SolvingAdminId = int.Parse(userIdClaim);
                var updatedProblem = await _problemService.AddAdminCommentAsync(id, commentDto);
                return Ok(updatedProblem);
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