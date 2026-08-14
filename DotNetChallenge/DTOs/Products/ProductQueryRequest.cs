using DotNetChallenge.DTOs.Common;

namespace DotNetChallenge.DTOs.Products
{
    public class ProductQueryRequest : PaginationRequest
    {
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
