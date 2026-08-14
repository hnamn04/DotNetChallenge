using DotNetChallenge.DTOs.SalesOrders;

namespace DotNetChallenge.Services.SalesOrders
{
    public interface ISalesOrderService
    {
        Task<SalesOrderResponse> CreateAsync(CreateSalesOrderRequest request);
        Task<List<SalesOrderResponse>> GetAllAsync();
        Task<SalesOrderResponse> GetByIdAsync(Guid id);
        Task<SalesOrderResponse> ConfirmAsync(Guid id);
        Task<SalesOrderResponse> CancelAsync(Guid id);
    }
}
