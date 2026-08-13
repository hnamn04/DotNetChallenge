using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.DTOs.Inventories
{
    public class StockTransactionResponse
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public StockTransactionType Type { get; set; }

        public int Quantity { get; set; }

        public string? ReferenceType { get; set; }

        public Guid? ReferenceId { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
