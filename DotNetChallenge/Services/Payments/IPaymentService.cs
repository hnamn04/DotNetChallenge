using DotNetChallenge.DTOs.Payments;

namespace DotNetChallenge.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreateAsync(Guid salesOrderId, CreatePaymentRequest request);

        Task<List<PaymentResponse>> GetBySalesOrderIdAsync(Guid salesOrderId);
    }
}
