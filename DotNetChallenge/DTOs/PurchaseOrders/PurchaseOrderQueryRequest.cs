using DotNetChallenge.Models.Enums;
using DotNetChallenge.DTOs.Common;

namespace DotNetChallenge.DTOs.PurchaseOrders
{
    public class PurchaseOrderQueryRequest : PaginationRequest
    {
        public  PurchaseOrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
