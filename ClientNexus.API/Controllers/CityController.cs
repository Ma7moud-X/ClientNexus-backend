using Amazon.S3.Model;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityServicecs _cityService;
        private readonly IUnitOfWork unitOfWork;
        // Constructor to inject the ICityService dependency
        public CityController(ICityServicecs cityService , IUnitOfWork unitOfWork)
        {
            _cityService = cityService;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]

        public IActionResult gat()
        {
            return Ok(unitOfWork.Cities.GetAllQueryable());
        }

        // POST api/city
        [HttpPost]
        //[Authorize(Roles = "Admin")]

        public async Task<ActionResult<ApiResponseDTO<CityDTO>>> AddCity([FromBody] CityDTO cityDTO)
        {
            try
            {
               

                // Call the AddCityAsync method from the CityService to add the city
                await _cityService.AddCityAsync (cityDTO);

                // Return a success response
                return Ok(ApiResponseDTO<CityDTO>.SuccessResponse(cityDTO, "City added successfully."));
            }
            catch (ArgumentException ex)
            {
                // If there is an argument error, return a BadRequest response
                return BadRequest(ApiResponseDTO<CityDTO>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                // If the city already exists, return a Conflict response
                return Conflict(ApiResponseDTO<CityDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors and return a ServerError response
                return StatusCode(500, ApiResponseDTO<CityDTO>.ErrorResponse($"An unexpected error occurred: {ex.Message}"));
            }
        }

        // DELETE api/city/{id}
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]

        public async Task<ActionResult<ApiResponseDTO<object>>> DeleteCity(int id)
        {
            try
            {
                // Call the DeleteCityAsync method from the CityService to delete the city
                await _cityService.DeleteCityAsync(id);

                // Return a success response
                return Ok(ApiResponseDTO<object>.SuccessResponse(null, "City deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                // If the city is not found, return a NotFound response
                return NotFound(ApiResponseDTO<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors and return a ServerError response
                return StatusCode(500, ApiResponseDTO<object>.ErrorResponse($"An unexpected error occurred: {ex.Message}"));
            }
        }
    }
}
