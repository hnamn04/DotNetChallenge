namespace DotNetChallenge.DTOs.Reports
{
    public class LowStockResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
