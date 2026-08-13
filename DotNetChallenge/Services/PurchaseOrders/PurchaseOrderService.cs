using DotNetChallenge.Data;
using DotNetChallenge.DTOs.PurchaseOrders;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Entities;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.PurchaseOrders
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _context;

        public PurchaseOrderService(AppDbContext context)
        {
            _context = context;
        }

        // Create a new purchase order
        public async Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderRequest request)
        {
            // Check if the supplier exists
            var supplierExists = await _context.Suppliers
                .AnyAsync(x => x.Id == request.SupplierId);

            // If the supplier does not exist, throw a NotFoundException
            if (!supplierExists)
            {
                throw new NotFoundException($"Supplier with id '{request.SupplierId}' was not found");
            }

            // Check products existence
            var productIds = request.Items
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            // Check if all product IDs exist in the database
            var existingProductIds = await _context.Products
                .Where(x => productIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            // Find the first product ID that does not exist in the database
            var invalidProductId = productIds
                .FirstOrDefault(x => !existingProductIds.Contains(x));

            // If an invalid product ID is found, throw a NotFoundException
            if (invalidProductId != Guid.Empty)
            {
                throw new NotFoundException($"Product with id '{invalidProductId}' was not found.");
            }

            // Create the purchase order
            var order = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                SupplierId = request.SupplierId,
                OrderDate = DateTime.UtcNow,
                Status = PurchaseOrderStatus.Draft
            };

            // Create the purchase order items
            foreach (var item in request.Items)
            {
                var orderItem = new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice
                };

                order.Items.Add(orderItem);
            }

            // Calculate the total amount of the purchase order
            order.TotalAmount = order.Items.Sum(x => x.TotalPrice);

            _context.PurchaseOrders.Add(order);

            await _context.SaveChangesAsync();

            return MapToResponse(order);
        }

        // Get all purchase orders
        public async Task<List<PurchaseOrderResponse>> GetAllAsync()
        {
            var orders = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(x => x.Items)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        // Get a purchase order by id
        public async Task<PurchaseOrderResponse> GetByIdAsync(Guid id)
        {
            // Get the purchase order by id, including its items
            var order = await _context.PurchaseOrders
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            // If the purchase order is not found, throw a NotFoundException
            if (order is null)
            {
                throw new NotFoundException(
                    $"Purchase order with id '{id}' was not found.");
            }

            return MapToResponse(order);
        }

        // Confirm a purchase order
        public async Task<PurchaseOrderResponse> ConfirmAsync(Guid id)
        {
            // Start a database transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get the purchase order by id, including its items
                var order = await _context.PurchaseOrders
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                // If the purchase order is not found, throw a NotFoundException
                if (order is null)
                {
                    throw new NotFoundException($"Purchase order with id '{id}' was not found.");
                }

                // If the purchase order is not in draft status, throw a ConflictException
                if (order.Status != PurchaseOrderStatus.Draft)
                {
                    throw new ConflictException("Purchase order has already been processed.");
                }

                // Update inventory and create stock transactions for each item in the purchase order
                foreach (var item in order.Items)
                {
                    // Check if the inventory record for the product exists
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(
                            x => x.ProductId == item.ProductId);

                    // If the inventory record does not exist, create a new one; otherwise, update the existing record
                    if (inventory is null)
                    {
                        inventory = new Inventory
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            ReservedQuantity = 0
                        };

                        _context.Inventories.Add(inventory);
                    }
                    else
                    {
                        inventory.Quantity += item.Quantity;
                    }

                    // Create a stock transaction
                    var stockTransaction = new StockTransaction
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Type = StockTransactionType.StockIn,
                        Quantity = item.Quantity,
                        ReferenceType = "PurchaseOrder",
                        ReferenceId = order.Id,
                        Note = $"Stock in from purchase order {order.OrderNumber}",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.StockTransactions.Add(stockTransaction);
                }

                // Update the purchase order status to confirmed and set the updated timestamp
                order.Status = PurchaseOrderStatus.Confirmed;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapToResponse(order);
            }
            catch // If any exception occurs, roll back the transaction and rethrow the exception
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Cancel a purchase order
        public async Task<PurchaseOrderResponse> CancelAsync(Guid id)
        {
            // Get the purchase order by id, including its items
            var order = await _context.PurchaseOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            // If the purchase order is not found, throw a NotFoundException
            if (order is null)
            {
                throw new NotFoundException($"Purchase order with id '{id}' was not found.");
            }

            // If the purchase order is not in draft status, throw a ConflictException
            if (order.Status != PurchaseOrderStatus.Draft)
            {
                throw new ConflictException("Purchase order has already been processed.");
            }

            // Update the purchase order status to cancelled and set the updated timestamp
            order.Status = PurchaseOrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(order);
        }


        private static PurchaseOrderResponse MapToResponse(PurchaseOrder purchaseOrder)
        {
            return new PurchaseOrderResponse
            {
                Id = purchaseOrder.Id,
                SupplierId = purchaseOrder.SupplierId,
                Status = purchaseOrder.Status,
                CreatedAt = purchaseOrder.CreatedAt,
                UpdatedAt = purchaseOrder.UpdatedAt,
                Items = purchaseOrder.Items
                    .Select(item => new PurchaseOrderItemResponse
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                    })
                    .ToList()
            };
        }

        private static string GenerateOrderNumber()
        {
            return $"PO-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
