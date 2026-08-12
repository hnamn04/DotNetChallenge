using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Products;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var code = request.Code.Trim();

            var codeExists = await _context.Products
                .AnyAsync(x => x.Code == code);

            if (codeExists)
            {
                throw new DuplicateProductCodeException(
                    $"Product code '{code}' already exists.");
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category is null)
            {
                throw new NotFoundException(
                    $"Category with id '{request.CategoryId}' was not found.");
            }

            var unit = await _context.Units
                .FirstOrDefaultAsync(x => x.Id == request.UnitId);

            if (unit is null)
            {
                throw new NotFoundException(
                    $"Unit with id '{request.UnitId}' was not found.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = request.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),
                CostPrice = request.CostPrice,
                SellingPrice = request.SellingPrice,
                CategoryId = request.CategoryId,
                UnitId = request.UnitId,
                Category = category,
                Unit = unit,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Unit)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return products.Select(MapToResponse);
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with id '{id}' was not found.");
            }

            return MapToResponse(product);
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with id '{id}' was not found.");
            }

            var code = request.Code.Trim();

            var codeExists = await _context.Products
                .AnyAsync(x =>
                    x.Code == code &&
                    x.Id != id);

            if (codeExists)
            {
                throw new DuplicateProductCodeException(
                    $"Product code '{code}' already exists.");
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category is null)
            {
                throw new NotFoundException(
                    $"Category with id '{request.CategoryId}' was not found.");
            }

            var unit = await _context.Units
                .FirstOrDefaultAsync(x => x.Id == request.UnitId);

            if (unit is null)
            {
                throw new NotFoundException(
                    $"Unit with id '{request.UnitId}' was not found.");
            }

            product.Code = code;
            product.Name = request.Name.Trim();
            product.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            product.CostPrice = request.CostPrice;
            product.SellingPrice = request.SellingPrice;
            product.CategoryId = request.CategoryId;
            product.UnitId = request.UnitId;
            product.Category = category;
            product.Unit = unit;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with id '{id}' was not found.");
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
        }

        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Description = product.Description,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                UnitId = product.UnitId,
                UnitName = product.Unit.Name,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
