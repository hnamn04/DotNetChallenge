using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Inventories;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Models.Entities;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Inventories
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        // Import
        public async Task<InventoryResponse> ImportAsync(InventoryImportRequest request)
        {
            var productExists = await _context.Products
                .AnyAsync(x => x.Id == request.ProductId);

            if (!productExists)
            {
                throw new NotFoundException($"Product with id '{request.ProductId}' was not found.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId);

                if (inventory is null)
                {
                    inventory = new Inventory
                    {
                        Id = Guid.NewGuid(),
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        ReservedQuantity = 0,
                        Version = Guid.NewGuid(), // Optimistic Concurrency
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Inventories.Add(inventory);
                }
                else
                {
                    inventory.Quantity += request.Quantity;
                    inventory.Version = Guid.NewGuid(); // Optimistic Concurrency
                    inventory.UpdatedAt = DateTime.UtcNow;
                }

                var stockTransaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.ProductId,
                    Type = StockTransactionType.StockIn,
                    Quantity = request.Quantity,
                    Note = request.Note.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockTransactions.Add(stockTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapToResponse(inventory);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                throw new ConflictException("Concurrent update detected or inventory already initialized.");
            }
        }

        // Export
        public async Task<InventoryResponse> ExportAsync(InventoryExportRequest request)
        {
            var productExists = await _context.Products
                .AnyAsync(x => x.Id == request.ProductId);

            if (!productExists)
            {
                throw new NotFoundException($"Product with id '{request.ProductId}' was not found.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(x => x.ProductId == request.ProductId);

                if (inventory is null)
                {
                    throw new ConflictException("Insufficient inventory.");
                }

                // Check available quantity
                var availableQuantity = inventory.Quantity - inventory.ReservedQuantity;

                if (request.Quantity > availableQuantity)
                {
                    throw new ConflictException($"Export quantity cannot exceed available inventory ({availableQuantity}).");
                }

                inventory.Quantity -= request.Quantity;
                inventory.Version = Guid.NewGuid(); // Trigger Optimistic Concurrency
                inventory.UpdatedAt = DateTime.UtcNow;

                var stockTransaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.ProductId,
                    Type = StockTransactionType.StockOut,
                    Quantity = request.Quantity,
                    Note = request.Note.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockTransactions.Add(stockTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapToResponse(inventory);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                // Handle concurrency exception
                throw new ConflictException("The inventory was modified by another process. Please try again.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get inventory by product id
        public async Task<InventoryResponse> GetByProductIdAsync(Guid productId)
        {
            var productExists = await _context.Products.AnyAsync(x => x.Id == productId);

            if (!productExists)
            {
                throw new NotFoundException($"Product with id '{productId}' was not found.");
            }

            var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (inventory is null)
            {
                return new InventoryResponse { ProductId = productId, Quantity = 0, ReservedQuantity = 0 };
            }

            return MapToResponse(inventory);
        }

        // Get paginated stock transactions
        public async Task<PaginatedList<StockTransactionResponse>> GetPagedTransactionsAsync(StockTransactionQueryRequest request)
        {
            var query = _context.StockTransactions
                .AsNoTracking()
                .AsQueryable();

            // Filter
            if (request.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == request.ProductId.Value);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(x => x.Type == request.Type.Value);
            }

            // Đếm tổng số
            var totalItems = await query.CountAsync();

            // Pagination
            var transactions = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .Select(x => new StockTransactionResponse
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Type = x.Type,
                    Quantity = x.Quantity,
                    ReferenceType = x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    Note = x.Note,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new PaginatedList<StockTransactionResponse>(transactions, request.Page, request.Limit, totalItems);
        }

        private static InventoryResponse MapToResponse(Inventory inventory)
        {
            return new InventoryResponse
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                ReservedQuantity = inventory.ReservedQuantity
            };
        }
    }
}