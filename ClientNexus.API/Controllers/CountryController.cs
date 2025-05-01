using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CountryController : ControllerBase
    {
        private readonly IcountryService countryService;

        public CountryController(IcountryService countryService)
        {
            this.countryService = countryService;
        }

        // POST api/country
        [HttpPost]
        //[Authorize(Policy = "IsAdmin")]

        public async Task<ActionResult<ApiResponseDTO<CountryDTO>>> AddCountry([FromBody] CountryDTO countryDTO)
        {
            try
            {


                await countryService.AddCountryAsync(countryDTO);
                return Ok(ApiResponseDTO<CountryDTO>.SuccessResponse(countryDTO, "Country added successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An unexpected error occurred: {ex.Message}"));
            }
        }

        // DELETE api/country/{id}
        [HttpDelete("{id}")]
        //[Authorize(Policy = "IsAdmin")]

        public async Task<ActionResult<ApiResponseDTO<object>>> DeleteCountry(int id)
        {
            try
            {
                await countryService.DeleteCountryAsync(id);
                return Ok(ApiResponseDTO<object>.SuccessResponse(null, "Country deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<object>.ErrorResponse($"An unexpected error occurred: {ex.Message}"));
            }
        }
    }
}
