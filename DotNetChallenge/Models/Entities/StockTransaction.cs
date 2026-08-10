using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.Models.Entities
{
    public class StockTransaction : BaseEntity
    {
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public StockTransactionType Type { get; set; }

        public int Quantity { get; set; }

        public string? ReferenceType { get; set; }

        public Guid? ReferenceId { get; set; }

        public string? Note { get; set; }
    }
}
