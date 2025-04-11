using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            this._clientService = clientService;
        }

        [HttpPut("{ClientId}")]
        public async Task<IActionResult> UpdateServiceProviderId(int ClientId, [FromBody] UpdateClientDTO updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest(ApiResponseDTO<string>.ErrorResponse("Invalid request data."));
            }

            try
            {
                await _clientService.UpdateClientAsync(ClientId, updateDto);
                return Ok(ApiResponseDTO<string>.SuccessResponse("Client updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
            }
        }
    }
}
