using ClientNexus.API.Utilities;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut]
        [Authorize(Policy = "IsClientOrAdmin")]
        public async Task<IActionResult> UpdateClient( [FromForm] UpdateClientDTO updateDto)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized(ApiResponseDTO<string>.ErrorResponse("user is not authorized."));


            try
            {

                await _clientService.UpdateClientAsync(userId.Value, updateDto);
                return Ok(ApiResponseDTO<string>.SuccessResponse("Client updated successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDTO<string>.ErrorResponse(ex.Message));
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
        [HttpGet]
        [Authorize(Policy = "IsClient")]
        public async Task<IActionResult> GetById(int? id )
        {
            int Id;
            if (id == null)
            {
                var userId = User.GetId();
                if (userId is null)
                    return Unauthorized(ApiResponseDTO<string>.ErrorResponse("User is not authorized."));
                Id= userId.Value;
            }
            else
            {
                Id = id.Value;
            }

            try
            {
                var response = await _clientService.GetByIdAsync(Id);

                if (response == null)
                    return NotFound(ApiResponseDTO<string>.ErrorResponse("Client not found."));

                // Wrap the response data in ApiResponseDTO
                return Ok(ApiResponseDTO<ClientResponseDTO>.SuccessResponse(response, "Client fetched successfully."));
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
