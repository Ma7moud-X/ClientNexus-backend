using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<DocumentResponseDTO>>> AddDocument([FromBody] DocumentDTO dto)
        {

            try
            {
                var result = await _documentService.AddDocumentAsync(dto);
                return Ok(ApiResponseDTO<DocumentResponseDTO>.SuccessResponse(result, "Document added successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<DocumentResponseDTO>.ErrorResponse("An unexpected error occurred: " + ex.Message));
            }
        }
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocumentAsync(int documentId)
        {
            try
            {
                // Call service to delete document and return a success response
                await _documentService.DeleteDocumentAsync(documentId);
                return Ok(ApiResponseDTO<object>.SuccessResponse(null, "Document deleted successfully."));
            }
            catch (KeyNotFoundException)
            {
                // If document not found, return an error response
                return NotFound(ApiResponseDTO<string>.ErrorResponse("Document not found."));
            }
            catch (Exception ex)
            {
                // If any exception occurs, return an error response
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred while deleting the document: {ex.Message}"));
            }
        }


    }
}
