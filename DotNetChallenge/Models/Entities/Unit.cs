namespace DotNetChallenge.Models.Entities
{
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string Symbol { get; set; } = null!;

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
