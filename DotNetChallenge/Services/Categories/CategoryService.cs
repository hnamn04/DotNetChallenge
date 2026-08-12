using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Categories;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return categories.Select(MapToResponse);
        }

        public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
            {
                throw new NotFoundException(
                    $"Category with id '{id}' was not found.");
            }

            category.Name = request.Name.Trim();
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
            {
                throw new NotFoundException(
                    $"Category with id '{id}' was not found.");
            }

            var hasProducts = await _context.Products
                .AnyAsync(x => x.CategoryId == id);

            if (hasProducts)
            {
                throw new ConflictException(
                    "Cannot delete category because it is being used by one or more products.");
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();
        }

        private static CategoryResponse MapToResponse(Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
