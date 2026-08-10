namespace DotNetChallenge.Models.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();
    }
}
