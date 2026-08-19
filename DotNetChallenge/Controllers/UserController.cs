using DotNetChallenge.Common.Authorization;
using DotNetChallenge.DTOs.Roles;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public UserController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // POST: api/users/{id}/roles
        [HttpPost("{id}/roles")]
        [Authorize(Policy = PolicyConstants.AdminOnly)]
        public async Task<ActionResult<ApiResponse<object>>> AssignRole(Guid id, AssignRoleRequest request)
        {
            await _roleService.AssignRoleAsync(id, request);

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Role assigned successfully.",
                    null));
        }
    }
}
