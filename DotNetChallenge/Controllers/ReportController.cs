using DotNetChallenge.Common.Authorization;
using DotNetChallenge.DTOs.Reports;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Authorize(Policy = PolicyConstants.ReportAccess)]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService; 
        
        public ReportController(IReportService reportService) 
        { 
            _reportService = reportService; 
        }

        // GET: api/reports/revenue
        [HttpGet("revenue")]

        public async Task<ActionResult<ApiResponse<RevenueReportResponse>>> GetRevenue([FromQuery] RevenueReportRequest request) 
        { 
            var result = await _reportService.GetRevenueAsync(request); 
            
            return Ok(new ApiResponse<RevenueReportResponse>(true, "Revenue report retrieved successfully.", result)); 
        }

        // GET: api/reports/inventory-low-stock
        [HttpGet("inventory-low-stock")]
        public async Task<ActionResult<ApiResponse<List<LowStockResponse>>>> GetLowStock([FromQuery] LowStockRequest request) 
        { 
            var result = await _reportService.GetLowStockAsync(request); 
            
            return Ok(new ApiResponse<List<LowStockResponse>>(true, "Low stock report retrieved successfully.", result)); 
        }

        // GET: api/reports/sales/export
        [HttpGet("sales/export")]
        public async Task<IActionResult> ExportSales([FromQuery] SalesExportQueryRequest request) 
        { 
            var file = await _reportService.ExportSalesAsync(request); 
            
            return File(file, "text/csv", "sales-report.csv"); 
        }
    }
}
