using DotNetChallenge.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet] public ActionResult<ApiResponse<object>> GetHealth() 
        { 
            return Ok(new ApiResponse<object>(true, "API is running.", null)); 
        }
    }
}
