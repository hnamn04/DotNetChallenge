namespace DotNetChallenge.Models.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();
    }
}
