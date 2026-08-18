using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Jobs;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DotNetChallenge.Services.Jobs
{
    public class DailySummaryService : IDailySummaryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DailySummaryService> _logger;

        public DailySummaryService(AppDbContext context, ILogger<DailySummaryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DailySummaryResponse> GenerateDailySummaryAsync(DateOnly date)
        {
            // Get start and end of the selected day
            var fromDate = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var toDate = DateTime.SpecifyKind(date.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

            // Count Confirmed sales orders and sum their total amounts
            var salesOrdersQuery = _context.SalesOrders
                .AsNoTracking()
                .Where(x =>
                    x.Status == SalesOrderStatus.Confirmed &&
                    x.OrderDate >= fromDate &&
                    x.OrderDate < toDate);

            var totalSalesOrders = await salesOrdersQuery.CountAsync();
            var totalOrderRevenue = await salesOrdersQuery.SumAsync(x => x.TotalAmount);

            // Actual collected
            var totalActualCollected = await _context.Payments
                .AsNoTracking()
                .Where(x =>
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt < toDate)
                .SumAsync(x => x.Amount);

            // Count purchase orders
            var totalPurchaseOrders = await _context.PurchaseOrders
                .AsNoTracking()
                .CountAsync(x =>
                    x.OrderDate >= fromDate &&
                    x.OrderDate < toDate);

            // Count stock transactions
            var totalStockTransactions = await _context.StockTransactions
                .AsNoTracking()
                .CountAsync(x =>
                    x.CreatedAt >= fromDate &&
                    x.CreatedAt < toDate);

            var result = new DailySummaryResponse
            {
                Date = date,
                TotalSalesOrders = totalSalesOrders,
                TotalRevenue = totalOrderRevenue,
                TotalActualCollected = totalActualCollected, 
                TotalPurchaseOrders = totalPurchaseOrders,
                TotalStockTransactions = totalStockTransactions
            };

            _logger.LogInformation(
                "Daily summary generated for {Date}. " +
                "SalesOrders: {SalesOrders}, " +
                "OrderRevenue: {Revenue}, " +
                "ActualCollected: {ActualCollected}, " +
                "PurchaseOrders: {PurchaseOrders}, " +
                "StockTransactions: {StockTransactions}",
                date,
                result.TotalSalesOrders,
                result.TotalRevenue,
                result.TotalActualCollected,
                result.TotalPurchaseOrders,
                result.TotalStockTransactions);

            return result;
        }
    }
}