using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SpecializationController : ControllerBase
    {
        private readonly ISpecializationService specializationService;

        public SpecializationController(ISpecializationService specializationService)
        {
            this.specializationService = specializationService;
        }



        [HttpPost]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> AddNewSpecialization([FromBody] SpecializationDTO specializationDTO)
        {
            try
            {
                // Call the service to add specialization
                await specializationService.AddNewSpecializationAsync(specializationDTO);
                return Ok(new ApiResponseDTO<bool>(true, "Specialization added successfully.", true));
            }
            catch (ArgumentException ex)
            {
                // Handle specific exception and send detailed message
                return BadRequest(new ApiResponseDTO<bool>(false, ex.Message, false));
            }
            catch (InvalidOperationException ex)
            {
                // Handle if specialization already exists
                return Conflict(new ApiResponseDTO<bool>(false, ex.Message, false));
            }
            catch (Exception ex)
            {
                // Catch all other exceptions
                return StatusCode(500, new ApiResponseDTO<bool>(false, "An unexpected error occurred.", false));
            }
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> DeleteSpecialization([FromRoute] int id)
        {
            try
            {
                // Call the service to delete specialization
                await specializationService.DeleteSpecializationAsync(id);
                return Ok(new ApiResponseDTO<bool>(true, "Specialization deleted successfully.", true));
            }
            catch (KeyNotFoundException ex)
            {
                // Handle when specialization not found
                return NotFound(new ApiResponseDTO<bool>(false, ex.Message, false));
            }
            catch (Exception ex)
            {
                // Catch all other exceptions
                return StatusCode(500, new ApiResponseDTO<bool>(false, "An unexpected error occurred.", false));
            }
        }
        [HttpGet("GetAllSpecializations")]
        //[Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<ApiResponseDTO<List<SpecializationResponseDTO>>>> GetAllSpecializations()
        {
            try
            {
                var specializations = await specializationService.GetAllSpecializationsAsync();

                return Ok(ApiResponseDTO<List<SpecializationResponseDTO>>.SuccessResponse(specializations));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<List<SpecializationResponseDTO>>.ErrorResponse($"An error occurred: {ex.Message}"));
            }
        }

    }

}

