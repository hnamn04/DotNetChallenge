using DotNetChallenge.DTOs.Roles;

namespace DotNetChallenge.Services.Roles
{
    public interface IRoleService
    {
        Task<List<RoleResponse>> GetAllAsync();

        Task AssignRoleAsync(AssignRoleRequest request);
    }
}
