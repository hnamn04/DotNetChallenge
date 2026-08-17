using DotNetChallenge.Common.Authorization;
using DotNetChallenge.DTOs.SalesOrders;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.SalesOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Policy = PolicyConstants.CreateSalesOrder)]
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
        public async Task<ActionResult<ApiResponse<PaginatedList<SalesOrderResponse>>>> GetAll([FromQuery] SalesOrderQueryRequest request)
        {
            var result = await _salesOrderService
                .GetPagedAsync(request);

            return Ok(new ApiResponse<PaginatedList<SalesOrderResponse>>(true, "Sales orders retrieved successfully.", result));
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
        [Authorize(Policy = PolicyConstants.ManageOrders)]
        public async Task<ActionResult<SalesOrderResponse>> Confirm(Guid id)
        {
            var result = await _salesOrderService.ConfirmAsync(id);

            return Ok(result);
        }

        // POST: api/sales-orders/{id}/cancel
        [HttpPost("{id:guid}/cancel")]
        [Authorize(Policy = PolicyConstants.ManageOrders)]
        public async Task<ActionResult<SalesOrderResponse>> Cancel(Guid id)
        {
            var result = await _salesOrderService.CancelAsync(id);

            return Ok(result);
        }
    }
}
