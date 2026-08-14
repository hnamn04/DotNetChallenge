using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetChallenge.DTOs.SalesOrders;
using DotNetChallenge.Services.SalesOrders;

namespace DotNetChallenge.Controllers
{
    [Route("api/sales-orders")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;

        public SalesOrderController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        // POST: api/sales-orders
        [HttpPost]
        public async Task<ActionResult<SalesOrderResponse>> Create(CreateSalesOrderRequest request)
        {
            var result = await _salesOrderService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // GET: api/sales-orders
        [HttpGet]
        public async Task<ActionResult<List<SalesOrderResponse>>> GetAll()
        {
            var result = await _salesOrderService.GetAllAsync();

            return Ok(result);
        }

        // GET: api/sales-orders/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SalesOrderResponse>> GetById(Guid id)
        {
            var result = await _salesOrderService.GetByIdAsync(id);

            return Ok(result);
        }

        // POST: api/sales-orders/{id}/confirm
        [HttpPost("{id:guid}/confirm")]
        public async Task<ActionResult<SalesOrderResponse>> Confirm(Guid id)
        {
            var result = await _salesOrderService.ConfirmAsync(id);

            return Ok(result);
        }

        // POST: api/sales-orders/{id}/cancel
        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<SalesOrderResponse>> Cancel(Guid id)
        {
            var result = await _salesOrderService.CancelAsync(id);

            return Ok(result);
        }
    }
}
