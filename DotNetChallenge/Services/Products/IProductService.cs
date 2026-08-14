using DotNetChallenge.DTOs.Products;
using DotNetChallenge.Models.Common;

namespace DotNetChallenge.Services.Products
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest request);

        Task<IEnumerable<ProductResponse>> GetAllAsync();

        Task<ProductResponse> GetByIdAsync(Guid id);

        Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);

        Task DeleteAsync(Guid id);

        Task<PaginatedList<ProductResponse>> GetPagedAsync(ProductQueryRequest request);
    }
}
