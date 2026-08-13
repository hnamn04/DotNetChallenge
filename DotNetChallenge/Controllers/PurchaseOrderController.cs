using DotNetChallenge.DTOs.PurchaseOrders;
using DotNetChallenge.Services.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/purchase-orders")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        // POST: api/purchase-orders
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderResponse>> Create(CreatePurchaseOrderRequest request)
        {
            var result = await _purchaseOrderService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // GET: api/purchase-orders
        [HttpGet]
        public async Task<ActionResult<List<PurchaseOrderResponse>>> GetAll()
        {
            var result = await _purchaseOrderService.GetAllAsync();

            return Ok(result);
        }

        // GET: api/purchase-orders/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderResponse>> GetById(Guid id)
        {
            var result = await _purchaseOrderService.GetByIdAsync(id);

            return Ok(result);
        }

        // PUT: api/purchase-orders/{id}
        [HttpPost("{id:guid}/confirm")]
        public async Task<ActionResult<PurchaseOrderResponse>> Confirm(Guid id)
        {
            var result = await _purchaseOrderService.ConfirmAsync(id);

            return Ok(result);
        }

        // POST: api/purchase-orders/{id}/cancel
        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<PurchaseOrderResponse>> Cancel(Guid id)
        {
            var result = await _purchaseOrderService.CancelAsync(id);

            return Ok(result);
        }
    }
}
