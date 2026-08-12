using Microsoft.AspNetCore.Mvc;
using DotNetChallenge.DTOs.Categories;
using DotNetChallenge.Services.Categories;

namespace DotNetChallenge.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: /api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request)
        {
            var response = await _categoryService.CreateAsync(request);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // GET: /api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
        {
            var response = await _categoryService.GetAllAsync();

            return Ok(response);
        }

        // PUT: /api/categories/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CategoryResponse>> Update(Guid id, UpdateCategoryRequest request)
        {
            var response = await _categoryService.UpdateAsync(id, request);

            return Ok(response);
        }

        // DELETE: /api/categories/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);

            return NoContent();
        }
    }
}
