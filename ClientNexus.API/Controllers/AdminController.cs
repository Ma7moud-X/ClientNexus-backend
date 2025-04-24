using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmainService _admainService;
        public AdminController(IAdmainService admainService)
        {
            this._admainService = admainService;
        }
        
        [Authorize(Policy = "IsAdmin")]
        [HttpPut("approve/{serviceProviderId}")]
        public async Task<IActionResult> ApproveServiceProvider(int serviceProviderId)
        {
            try
            {
                await _admainService.ApprovingServiceProviderAsync(serviceProviderId);
                return Ok(ApiResponseDTO<string>.SuccessResponse("Service provider approved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
            }
        }



    }
}
