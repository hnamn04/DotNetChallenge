namespace DotNetChallenge.DTOs.SalesOrders
{
    public class CreateSalesOrderRequest
    {
        public Guid CustomerId { get; set; }

        public List<SalesOrderItemRequest> Items { get; set; } = new List<SalesOrderItemRequest>();
    }
}
