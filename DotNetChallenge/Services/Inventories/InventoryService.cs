using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Inventories;
using DotNetChallenge.Exceptions;
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
                throw new NotFoundException(
                    $"Product with id '{request.ProductId}' was not found.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(
                        x => x.ProductId == request.ProductId);

                if (inventory is null)
                {
                    inventory = new Inventory
                    {
                        Id = Guid.NewGuid(),
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        ReservedQuantity = 0,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Inventories.Add(inventory);
                }
                else
                {
                    inventory.Quantity += request.Quantity;
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
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Export
        public async Task<InventoryResponse> ExportAsync(InventoryExportRequest request)
        {
            var productExists = await _context.Products
                .AnyAsync(x => x.Id == request.ProductId);

            if (!productExists)
            {
                throw new NotFoundException(
                    $"Product with id '{request.ProductId}' was not found.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(
                        x => x.ProductId == request.ProductId);

                if (inventory is null)
                {
                    throw new ConflictException(
                        "Insufficient inventory.");
                }

                if (request.Quantity > inventory.Quantity)
                {
                    throw new ConflictException(
                        "Export quantity cannot exceed current inventory.");
                }

                inventory.Quantity -= request.Quantity;
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
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get inventory by product id
        public async Task<InventoryResponse> GetByProductIdAsync(Guid productId)
        {
            var productExists = await _context.Products
                .AnyAsync(x => x.Id == productId);

            if (!productExists)
            {
                throw new NotFoundException(
                    $"Product with id '{productId}' was not found.");
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (inventory is null)
            {
                return new InventoryResponse
                {
                    ProductId = productId,
                    Quantity = 0,
                    ReservedQuantity = 0
                };
            }

            return MapToResponse(inventory);
        }

        // Get all stock transactions
        public async Task<IEnumerable<StockTransactionResponse>>GetTransactionsAsync()
        {
            var transactions = await _context.StockTransactions
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
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

            return transactions;
        }

        // Map Inventory entity to InventoryResponse DTO
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
