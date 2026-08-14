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
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoleResponse>>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();

            return Ok(
                new ApiResponse<List<RoleResponse>>(
                    true,
                    "Roles retrieved successfully.",
                    roles));
        }

        // POST: api/roles/{id}/roles
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>>AssignRole(AssignRoleRequest request)
        {
            await _roleService.AssignRoleAsync(request);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Role assigned successfully.",
                    null));
        }
    }
}
