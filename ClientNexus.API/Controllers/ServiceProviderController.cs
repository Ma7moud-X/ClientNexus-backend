using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
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

        [HttpGet("Search")]
        public async Task<IActionResult> SearchServiceProviders([FromQuery] string? searchQuery)
            {



                try
                {
                    var providers = await _serviceProviderIsService.SearchServiceProvidersAsync(searchQuery);

                    if (!providers.Any())
                    {
                        return NotFound(ApiResponseDTO<List<ServiceProviderResponseDTO>>.ErrorResponse("No matching service providers found."));
                    }

                    return Ok(ApiResponseDTO<List<ServiceProviderResponseDTO>>.SuccessResponse(providers, "Service providers retrieved successfully."));
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
                        return NotFound(ApiResponseDTO<List<ServiceProviderResponseDTO>>.ErrorResponse("No matching service providers found."));
                    }

                    return Ok(ApiResponseDTO<List<ServiceProviderResponseDTO>>.SuccessResponse(providers, "Service providers retrieved successfully."));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
                }


            }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isApproved)
        {
            try
            {
                var result = await _serviceProviderIsService.GetAllServiceProvider(isApproved);

                if (result == null || !result.Any())
                {
                    return NotFound(ApiResponseDTO<List<ServiceProviderResponseDTO>>
                        .ErrorResponse("No matching service providers found."));
                }

                return Ok(ApiResponseDTO<List<ServiceProviderResponseDTO>>
                    .SuccessResponse(result, "Service providers retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>
                    .ErrorResponse($"An error occurred: {ex.Message}"));
            }
        }


            
        }

    }

