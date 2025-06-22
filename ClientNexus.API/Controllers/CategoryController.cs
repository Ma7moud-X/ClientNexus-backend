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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpPost]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> AddCategory([FromBody] string categoryName)
        {
            try
            {
                var result = await _categoryService.AddCategoryAsync(categoryName);

                var response = ApiResponseDTO<CategoryResponseDTO>.SuccessResponse(result, "Category added successfully.");
                return Ok(response);
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


        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return Ok(ApiResponseDTO<string>.SuccessResponse(null, "Category deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }

            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse("An unexpected error occurred."));
            }
        }
        //[Authorize(Policy = "IsAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<StateResponseDTO>>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllStatesAsync();

                return Ok(ApiResponseDTO<List<CategoryResponseDTO>>.SuccessResponse(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<List<StateResponseDTO>>.ErrorResponse($"An error occurred: {ex.Message}"));
            }
        }



    }
}
