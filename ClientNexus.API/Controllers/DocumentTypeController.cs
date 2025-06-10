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
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService documentTypeService;

        public DocumentTypeController(IDocumentTypeService documentTypeService)
        {
            this.documentTypeService = documentTypeService;
        }

        [HttpPost]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> AddDocumentTypeService([FromBody] DocumentTypeDTO documentTypeDTO)
        {
            try
            {
                await documentTypeService.AddDocumentTypeAsync(documentTypeDTO);


                return Ok(ApiResponseDTO<object>.SuccessResponse(null, $"Document Type '{documentTypeDTO.Name}' added successfully."));

            }
            catch (ArgumentException ex)
            {
                var response = ApiResponseDTO<string>.ErrorResponse(ex.Message);
                return BadRequest(response);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponseDTO<string>.ErrorResponse(ex.Message);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponseDTO<string>.ErrorResponse("An unexpected error occurred.");
                return StatusCode(500, response);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<ApiResponseDTO<object>>> DeleteDocumentTypeAsyn(int id)
        {
            try
            {
                await documentTypeService.DeleteDocumentTypeAsync(id);
                return Ok(ApiResponseDTO<object>.SuccessResponse(null, "Document Type deleted successfully."));
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
