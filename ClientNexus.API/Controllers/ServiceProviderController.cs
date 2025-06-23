using ClientNexus.API.Utilities;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;

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

        //[Authorize(Policy = "IsClientOrAdmin")]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchServiceProviders([FromQuery] string? searchQuery)
            {



                try
                {
                    var providers = await _serviceProviderIsService.SearchServiceProvidersAsync(searchQuery);

                if (!providers.Any())
                {
                    var successResponse = ApiResponseDTO<List<ServiceProviderResponseDTO>>.SuccessResponse(new List<ServiceProviderResponseDTO>(), "No matching service providers found.");
                    return Ok(successResponse);
                }

                return Ok(ApiResponseDTO<List<ServiceProviderResponseDTO>>.SuccessResponse(providers, "Service providers retrieved successfully."));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
                }
            }

        //[Authorize(Policy = "IsClientOrAdmin")]
        [HttpGet("filter")]
            public async Task<IActionResult> FilterServiceProviders([FromQuery] string? searchQuery, [FromQuery] float? minRate, [FromQuery] string? state, [FromQuery] string? city, [FromQuery] string? specializationName)
            {
                try
                {
                    var providers = await _serviceProviderIsService.FilterServiceProviderResponses(searchQuery, minRate, state, city, specializationName);

                if (!providers.Any())
                {
                    var successResponse = ApiResponseDTO<List<ServiceProviderResponseDTO>>.SuccessResponse(new List<ServiceProviderResponseDTO>(), "No matching service providers found.");
                    return Ok(successResponse);
                }

                return Ok(ApiResponseDTO<List<ServiceProviderResponseDTO>>.SuccessResponse(providers, "Service providers retrieved successfully."));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
                }


            }
        //[Authorize(Policy = "IsAdmin")]
        [HttpGet("GetAll")]
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




        [Authorize(Policy = "IsServiceProviderOrAdmin")]
        [HttpPut]
            public async Task<IActionResult> UpdateServiceProviderId( [FromForm] UpdateServiceProviderDTO updateDto)
            {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized(ApiResponseDTO<string>.ErrorResponse("user is not authorized."));

            if (updateDto == null)
                {
                    return BadRequest(ApiResponseDTO<string>.ErrorResponse("Invalid request data."));
                }

                try
                {
                    await _serviceProviderIsService.UpdateServiceProviderAsync(userId.Value, updateDto);
                    return Ok(ApiResponseDTO<string>.SuccessResponse("serviceprovider updated successfully."));
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(int? id)
        {

            int Id;
            if (id == null)
            {
                var userId = User.GetId();
                if (userId is null)
                    return Unauthorized(ApiResponseDTO<string>.ErrorResponse("User is not authorized."));
                Id = userId.Value;
            }
            else
            {
                Id = id.Value;
            }

           
            try
            {
                var response = await _serviceProviderIsService.GetByIdAsync(Id);

                if (response == null)
                    return NotFound(ApiResponseDTO<string>.ErrorResponse("ServiceProvider not found."));

                // Wrap the response data in ApiResponseDTO
                return Ok(ApiResponseDTO<ServiceProviderResponseDTO>.SuccessResponse(response, "ServiceProvider fetched successfully."));
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

