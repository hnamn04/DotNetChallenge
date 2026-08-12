using DotNetChallenge.DTOs.Products;

namespace DotNetChallenge.Services.Products
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest request);

        Task<IEnumerable<ProductResponse>> GetAllAsync();

        Task<ProductResponse> GetByIdAsync(Guid id);

        Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);

        Task DeleteAsync(Guid id);
    }
}
