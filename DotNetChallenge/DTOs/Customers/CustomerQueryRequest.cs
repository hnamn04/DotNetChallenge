using DotNetChallenge.DTOs.Common;

namespace DotNetChallenge.DTOs.Customers
{
    public class CustomerQueryRequest : PaginationRequest
    {
        public string? Search { get; set; }
    }
}
