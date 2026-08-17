using DotNetChallenge.Common.Authorization;
using DotNetChallenge.DTOs.Roles;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/roles
        [HttpGet]
        [Authorize(Policy = PolicyConstants.AdminOnly)]
        public async Task<ActionResult<ApiResponse<List<RoleResponse>>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();

            return Ok(
                new ApiResponse<List<RoleResponse>>(
                    true,
                    "Roles retrieved successfully.",
                    roles));
        }
    }
}
