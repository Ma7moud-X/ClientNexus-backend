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
        [HttpPost("Country")]
        public async Task<IActionResult> AddCountry([FromBody] CountryDTO countryDTO)
        {
            await _admainService.AddCountryAsync(countryDTO);
            return Ok("Country created successfully");

        }

        [HttpDelete("country/{id:int}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _admainService.DeleteCountryAsync(id);
            return Ok("Country deleted successfully");

        }
        [HttpPost("City")]
        public async Task<IActionResult> AddCity([FromBody] CityDTO cityDTO)
        {
            await _admainService.AddCityAsync(cityDTO);
            return Ok("City created successfully");

        }
        [HttpDelete("City/{id:int}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            await _admainService.DeleteCityAsync(id);
            return Ok("City deleted successfully");

        }
        [HttpPost("State")]
        public async Task<IActionResult> AddState([FromBody] StateDTO stateDTO)
        {
            await _admainService.AddStateAsync(stateDTO);
            return Ok("State created successfully");

        }
        [HttpDelete("state/{id:int}")]
        public async Task<IActionResult> DeleteState(int id)
        {
            await _admainService.DeleteStateAsync(id);
            return Ok("state deleted successfully");

        }
        [HttpPost("ServiceProviderType")]
        public async Task<IActionResult> AddServiceProviderType([FromBody] string Name)
        {
            await _admainService.AddServiceProviderTypeAsyn(Name);
            return Ok("Service Provider Type created successfully");

        }
        [HttpDelete("ServiceProviderType/{Id:int}")]
        public async Task<IActionResult> DeleteServiceProviderType(int Id)
        {
            await _admainService.DeleteServiceProviderTypeAsync(Id);
            return Ok("Service Provider Type deleted successfully");
        }

        [HttpPost("Specialization")]
        public async Task<IActionResult> AddSpecialization([FromBody] SpecializationDTO specializationDTO)
        {
            await _admainService.AddSpecializationAsync(specializationDTO);
            return Ok("Specialization created successfully");

        }
        [HttpDelete("Specialization/{Id:int}")]
        public async Task<IActionResult> DeleteSpecialization(int Id)
        {
            await _admainService.DeleteSpecializationAsync(Id);
            return Ok("Specialization deleted successfully");
        }

    }
}
        [HttpDelete("country/{id:int}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _admainService.DeleteCountryAsync(id);
            return Ok("Country deleted successfully");

        }
        [HttpPost("City")]
        public async Task<IActionResult> AddCity([FromBody] CityDTO cityDTO)
        {
            await _admainService.AddCityAsync(cityDTO);
            return Ok("City created successfully");

        }
        [HttpDelete("City/{id:int}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            await _admainService.DeleteCityAsync(id);
            return Ok("City deleted successfully");

        }
        [HttpPost("State")]
        public async Task<IActionResult> AddState([FromBody] StateDTO stateDTO)
        {
            await _admainService.AddStateAsync(stateDTO);
            return Ok("State created successfully");

        }
        [HttpDelete("state/{id:int}")]
        public async Task<IActionResult> DeleteState(int id)
        {
            await _admainService.DeleteStateAsync(id);
            return Ok("state deleted successfully");

        }
        [HttpPost("ServiceProviderType")]
        public async Task<IActionResult> AddServiceProviderType([FromBody] string Name)
        {
            await _admainService.AddServiceProviderTypeAsyn(Name);
            return Ok("Service Provider Type created successfully");

        }
        [HttpDelete("ServiceProviderType/{Id:int}")]
        public async Task<IActionResult> DeleteServiceProviderType(int Id)
        {
            await _admainService.DeleteServiceProviderTypeAsync(Id);
            return Ok("Service Provider Type deleted successfully");
        }

        [HttpPost("Specialization")]
        public async Task<IActionResult> AddSpecialization([FromBody] SpecializationDTO specializationDTO)
        {
            await _admainService.AddSpecializationAsync(specializationDTO);
            return Ok("Specialization created successfully");

        }
        [HttpDelete("Specialization/{Id:int}")]
        public async Task<IActionResult> DeleteSpecialization(int Id)
        {
            await _admainService.DeleteSpecializationAsync(Id);
            return Ok("Specialization deleted successfully");
        }

    }
}
}



