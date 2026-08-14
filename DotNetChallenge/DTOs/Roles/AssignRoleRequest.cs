namespace DotNetChallenge.DTOs.Roles
{
    public class AssignRoleRequest
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
