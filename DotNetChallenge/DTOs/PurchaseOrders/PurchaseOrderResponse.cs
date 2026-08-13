using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.DTOs.PurchaseOrders
{
    public class PurchaseOrderResponse
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PurchaseOrderItemResponse> Items { get; set; } = [];
        public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
    }
}
