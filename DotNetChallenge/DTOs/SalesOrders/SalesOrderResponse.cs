using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.DTOs.SalesOrders
{
    public class SalesOrderResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public SalesOrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SalesOrderItemResponse> Items { get; set; } = new List<SalesOrderItemResponse>();
    }
}
