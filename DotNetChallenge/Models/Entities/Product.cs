namespace DotNetChallenge.Models.Entities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public Guid UnitId { get; set; }

        public Unit Unit { get; set; } = null!;

        public Inventory? Inventory { get; set; }

        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();

        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
