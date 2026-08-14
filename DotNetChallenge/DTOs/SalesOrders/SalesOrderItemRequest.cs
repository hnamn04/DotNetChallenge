namespace DotNetChallenge.DTOs.SalesOrders
{
    public class SalesOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
