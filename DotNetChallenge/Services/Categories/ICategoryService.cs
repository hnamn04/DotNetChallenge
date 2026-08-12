using DotNetChallenge.DTOs.Categories;

namespace DotNetChallenge.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);

        Task<IEnumerable<CategoryResponse>> GetAllAsync();

        Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request);

        Task DeleteAsync(Guid id);
    }
}
