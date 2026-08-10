namespace DotNetChallenge.Models.Entities
{
    public class Inventory : BaseEntity
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; } = 0;

        public int ReservedQuantity { get; set; } = 0;

        public Product Product { get; set; } = null!;
    }
}
