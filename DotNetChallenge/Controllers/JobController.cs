using DotNetChallenge.DTOs.Jobs;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IDailySummaryService _dailySummaryService; 

        public JobController(IDailySummaryService dailySummaryService) 
        { 
            _dailySummaryService = dailySummaryService; 
        }

        // POST: api/jobs/daily-summary/run
        [HttpPost("daily-summary/run")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<ApiResponse<DailySummaryResponse>>> RunDailySummary() 
        { 
            var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); 

            var result = await _dailySummaryService.GenerateDailySummaryAsync(date); 
            
            return Ok(new ApiResponse<DailySummaryResponse>(true, "Daily summary job completed successfully.", result)); 
        }
    }
}
