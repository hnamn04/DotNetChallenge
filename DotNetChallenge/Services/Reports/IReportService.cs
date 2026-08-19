using DotNetChallenge.DTOs.Reports;

namespace DotNetChallenge.Services.Reports
{
    public interface IReportService
    {
        Task<RevenueReportResponse> GetRevenueAsync(RevenueReportRequest request);
        Task<List<LowStockResponse>> GetLowStockAsync(LowStockRequest request);
        Task<byte[]> ExportSalesAsync(SalesExportQueryRequest request);
    }
}
