using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.Models.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public string OrderNumber { get; set; } = null!;
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
        public decimal TotalAmount { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}
