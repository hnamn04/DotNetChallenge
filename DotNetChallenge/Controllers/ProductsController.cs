using Microsoft.AspNetCore.Mvc;
using DotNetChallenge.DTOs.Products;
using DotNetChallenge.Services.Products;

namespace DotNetChallenge.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // POST: /api/products
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
        {
            var response = await _productService.CreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        
        // GET: /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
        {
            var response = await _productService.GetAllAsync();

            return Ok(response);
        }

        // GET: /api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetById(Guid id)
        {
            var response = await _productService.GetByIdAsync(id);

            return Ok(response);
        }

        // PUT: /api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request)
        {
            var response = await _productService.UpdateAsync(id, request);

            return Ok(response);
        }

        // DELETE: /api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);

            return NoContent();
        }
    }
}
