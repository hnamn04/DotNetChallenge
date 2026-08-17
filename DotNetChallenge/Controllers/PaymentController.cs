using DotNetChallenge.Common.Authorization;
using DotNetChallenge.DTOs.Payments;
using DotNetChallenge.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/sales-orders/{salesOrderId:guid}/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/sales-orders/{salesOrderId}/payments
        [HttpPost]
        [Authorize(Policy = PolicyConstants.PaymentAccess)]
        public async Task<ActionResult<PaymentResponse>> Create(Guid salesOrderId, CreatePaymentRequest request)
        {
            var result = await _paymentService.CreateAsync(salesOrderId, request);

            return Ok(result);
        }

        // GET: api/sales-orders/{salesOrderId}/payments
        [HttpGet]
        [Authorize(Policy = PolicyConstants.PaymentAccess)]
        public async Task<ActionResult<List<PaymentResponse>>> GetAll(Guid salesOrderId)
        {
            var result = await _paymentService
                .GetBySalesOrderIdAsync(salesOrderId);

            return Ok(result);
        }
    }
}
