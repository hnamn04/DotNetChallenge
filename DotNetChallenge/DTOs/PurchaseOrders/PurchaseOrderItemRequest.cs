namespace DotNetChallenge.DTOs.PurchaseOrders
{
    public class PurchaseOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
