using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.Models.Entities
{
    public class SalesOrder : BaseEntity
    {
        public string OrderNumber { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;
        public decimal TotalAmount { get; set; }
        public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
