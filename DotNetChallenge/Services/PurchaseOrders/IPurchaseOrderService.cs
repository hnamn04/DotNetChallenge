using DotNetChallenge.DTOs.PurchaseOrders;

namespace DotNetChallenge.Services.PurchaseOrders
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request);
        Task<List<PurchaseOrderResponse>> GetAllAsync();
        Task<PurchaseOrderResponse> GetByIdAsync(Guid id);
        Task<PurchaseOrderResponse> ConfirmAsync(Guid id);
        Task<PurchaseOrderResponse> CancelAsync(Guid id);
    }
}
