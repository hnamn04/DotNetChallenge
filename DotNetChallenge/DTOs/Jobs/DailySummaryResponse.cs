namespace DotNetChallenge.DTOs.Jobs
{
    public class DailySummaryResponse
    {
        public DateOnly Date { get; set; } 
        public int TotalSalesOrders { get; set; } 
        public decimal TotalRevenue { get; set; } 
        public int TotalPurchaseOrders { get; set; } 
        public int TotalStockTransactions { get; set; } 
    }
}
