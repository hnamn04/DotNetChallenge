using Microsoft.AspNetCore.Mvc;
using DotNetChallenge.DTOs.Units;
using DotNetChallenge.Services.Units;

namespace DotNetChallenge.Controllers
{
    [ApiController]
    [Route("api/units")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitsController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        // POST: /api/units
        [HttpPost]
        public async Task<ActionResult<UnitResponse>> Create(CreateUnitRequest request)
        {
            var response = await _unitService.CreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }

        // GET: /api/units
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitResponse>>> GetAll()
        {
            var response = await _unitService.GetAllAsync();

            return Ok(response);
        }

        // PUT: /api/units/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UnitResponse>> Update(Guid id, UpdateUnitRequest request)
        {
            var response = await _unitService.UpdateAsync(id, request);

            return Ok(response);
        }
        
        // DELETE: /api/units/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _unitService.DeleteAsync(id);

            return NoContent();
        }
    }
}
