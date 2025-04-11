using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        
        
            private readonly IServiceProviderService _serviceProviderIsService;

            public ServiceProviderController(IServiceProviderService serviceProviderIsService)
            {
                this._serviceProviderIsService = serviceProviderIsService;
            }

            [HttpGet]
            public async Task<IActionResult> SearchServiceProviders([FromQuery] string? searchQuery)
            {



                try
                {
                    var providers = await _serviceProviderIsService.SearchServiceProvidersAsync(searchQuery);

                    if (!providers.Any())
                    {
                        return NotFound(ApiResponseDTO<List<ServiceProviderResponse>>.ErrorResponse("No matching service providers found."));
                    }

                    return Ok(ApiResponseDTO<List<ServiceProviderResponse>>.SuccessResponse(providers, "Service providers retrieved successfully."));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
                }
            }

            [HttpGet("filter")]
            public async Task<IActionResult> FilterServiceProviders([FromQuery] string? searchQuery, [FromQuery] float? minRate, [FromQuery] string? state, [FromQuery] string? city, [FromQuery] string? specializationName)
            {
                try
                {
                    var providers = await _serviceProviderIsService.FilterServiceProviderResponses(searchQuery, minRate, state, city, specializationName);

                    if (!providers.Any())
                    {
                        return NotFound(ApiResponseDTO<List<ServiceProviderResponse>>.ErrorResponse("No matching service providers found."));
                    }

                    return Ok(ApiResponseDTO<List<ServiceProviderResponse>>.SuccessResponse(providers, "Service providers retrieved successfully."));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
                }


            }



            [HttpPut("{ServiceProviderId}")]
            public async Task<IActionResult> UpdateServiceProviderId(int ServiceProviderId, [FromBody] UpdateServiceProviderDTO updateDto)
            {
                if (updateDto == null)
                {
                    return BadRequest(ApiResponseDTO<string>.ErrorResponse("Invalid request data."));
                }

                try
                {
                    await _serviceProviderIsService.UpdateServiceProviderAsync(ServiceProviderId, updateDto);
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

