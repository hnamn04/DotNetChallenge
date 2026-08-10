namespace DotNetChallenge.Models.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; }
            = new List<SalesOrder>();
    }
}
