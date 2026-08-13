namespace DotNetChallenge.DTOs.PurchaseOrders
{
    public class CreatePurchaseOrderRequest
    {
        public Guid SupplierId { get; set; }
        public List<PurchaseOrderItemRequest> Items { get; set; } = [];
    }
}
