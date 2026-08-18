using DotNetChallenge.DTOs.Common;
using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.DTOs.Inventories
{
    public class StockTransactionQueryRequest : PaginationRequest
    {
        public Guid? ProductId { get; set; }
        public StockTransactionType? Type { get; set; }
    }
}
