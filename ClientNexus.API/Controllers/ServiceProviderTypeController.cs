using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderTypeController : ControllerBase
    {
        private readonly IServiceProviderTypeService serviceProviderTypeService;

        public ServiceProviderTypeController(IServiceProviderTypeService serviceProviderTypeService)
        {
            this.serviceProviderTypeService = serviceProviderTypeService;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<object>>> AddServiceProviderType([FromBody] string name)
        {
            try
            {
                

                await serviceProviderTypeService.AddServiceProviderTypeAsyn(name);
                return Ok(ApiResponseDTO<object>.SuccessResponse(null, $"ServiceProviderType '{name}' added successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDTO<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<object>.ErrorResponse($"Unexpected error: {ex.Message}"));
            }
        }


    }
}
