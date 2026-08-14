using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Reports;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DotNetChallenge.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // Revenue report generation
        public async Task<RevenueReportResponse> GetRevenueAsync(RevenueReportRequest request)
        {
            // Convert date to DateTime range
            var fromDate = request.FromDate
                .ToDateTime(TimeOnly.MinValue);

            var toDate = request.ToDate
                .AddDays(1)
                .ToDateTime(TimeOnly.MinValue);

            // Get confirmed sales orders in the date range
            var query = _context.SalesOrders
                .AsNoTracking()
                .Where(x =>
                    x.Status == SalesOrderStatus.Confirmed &&
                    x.OrderDate >= fromDate &&
                    x.OrderDate < toDate);

            // Calculate total revenue
            var totalRevenue = await query
                .SumAsync(x => x.TotalAmount);

            // Count orders
            var totalOrders = await query
                .CountAsync();

            return new RevenueReportResponse
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders
            };
        }

        // Low stock report generation
        public async Task<List<LowStockResponse>> GetLowStockAsync(LowStockRequest request)
        {
            // Get products whose stock is below the threshold
            var inventories = await _context.Inventories
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Quantity <= request.Threshold)
                .OrderBy(x => x.Quantity)
                .ToListAsync();

            return inventories
                .Select(x => new LowStockResponse
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity
                })
                .ToList();
        }

        // Export sales orders to CSV
        public async Task<byte[]> ExportSalesAsync()
        {
            // Get sales orders
            var orders = await _context.SalesOrders
                .AsNoTracking()
                .OrderBy(x => x.OrderDate)
                .ToListAsync();

            var csv = new StringBuilder();

            // CSV header
            csv.AppendLine("OrderNumber,CustomerId,OrderDate,Status,TotalAmount");

            // CSV data
            foreach (var order in orders)
            {
                csv.AppendLine(
                    $"{order.OrderNumber}," +
                    $"{order.CustomerId}," +
                    $"{order.OrderDate:yyyy-MM-dd HH:mm:ss}," +
                    $"{order.Status}," +
                    $"{order.TotalAmount}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}
