using DotNetChallenge.Models.Enums;
using DotNetChallenge.DTOs.Common;

namespace DotNetChallenge.DTOs.SalesOrders
{
    public class SalesOrderQueryRequest : PaginationRequest
    {
        public SalesOrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
