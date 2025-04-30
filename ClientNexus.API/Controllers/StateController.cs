using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }


        [HttpPost]
        //[Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<ApiResponseDTO<StateDTO>>> AddState([FromBody] StateDTO stateDTO)
        {
            try
            {
                if (stateDTO == null)
                    return BadRequest(ApiResponseDTO<StateDTO>.ErrorResponse("State data is required."));

                await _stateService.AddStateAsync(stateDTO);
                return Ok(ApiResponseDTO<StateDTO>.SuccessResponse(stateDTO, "State added successfully."));
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

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]

        public async Task<ActionResult<ApiResponseDTO<object>>> DeleteState(int id)
        {
            try
            {
                await _stateService.DeleteStateAsync(id);
                return Ok(ApiResponseDTO<object>.SuccessResponse(null, "State deleted successfully."));
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
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<StateResponseDTO>>>> GetAllStates()
        {
            try
            {
                var states = await _stateService.GetAllStatesAsync(); 

                return Ok(ApiResponseDTO<List<StateResponseDTO>>.SuccessResponse(states)); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<List<StateResponseDTO>>.ErrorResponse($"An error occurred: {ex.Message}")); 
            }
        }

    }
}
