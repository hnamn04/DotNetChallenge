using DotNetChallenge.Data;
using DotNetChallenge.DTOs.SalesOrders;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Models.Entities;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.SalesOrders
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly AppDbContext _context;

        public SalesOrderService(AppDbContext context)
        {
            _context = context;
        }

        // Create a new sales order
        public async Task<SalesOrderResponse> CreateAsync(CreateSalesOrderRequest request)
        {
            // Check if the customer exists
            var customerExists = await _context.Customers
                .AnyAsync(x => x.Id == request.CustomerId);

            // If the customer does not exist, throw a NotFoundException
            if (!customerExists)
            {
                throw new NotFoundException($"Customer with id '{request.CustomerId}' was not found.");
            }

            // Check if the request contains any items
            var productIds = request.Items
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            // Get products from the database
            var products = await _context.Products
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync();

            // Check if all product IDs in the request are valid
            var invalidProductId = productIds
                .FirstOrDefault(x => !products.Any(p => p.Id == x));

            // If there is an invalid product ID, throw a NotFoundException
            if (invalidProductId != Guid.Empty)
            {
                throw new NotFoundException($"Product with id '{invalidProductId}' was not found.");
            }

            // Check if any product is inactive
            var inactiveProduct = products
                .FirstOrDefault(x => !x.IsActive);

            if (inactiveProduct is not null)
            {
                throw new ConflictException(
                    $"Product with id '{inactiveProduct.Id}' is inactive.");
            }

            // Create a new sales order
            var order = new SalesOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = SalesOrderStatus.Draft
            };

            // Create order items
            foreach (var item in request.Items)
            {
                var product = products
                    .First(x => x.Id == item.ProductId);

                var orderItem = new SalesOrderItem
                {
                    Id = Guid.NewGuid(),
                    SalesOrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,

                    // Save selling price at the time of sale
                    UnitPrice = product.SellingPrice,

                    TotalPrice =
                        product.SellingPrice * item.Quantity
                };

                order.Items.Add(orderItem);
            }

            // Calculate total amount
            order.TotalAmount = order.Items
                .Sum(x => x.TotalPrice);

            _context.SalesOrders.Add(order);

            await _context.SaveChangesAsync();

            return MapToResponse(order);
        }

        //// Get all sales orders
        //public async Task<List<SalesOrderResponse>> GetAllAsync()
        //{
        //    var orders = await _context.SalesOrders
        //        .AsNoTracking()
        //        .Include(x => x.Items)
        //        .OrderByDescending(x => x.OrderDate)
        //        .ToListAsync();

        //    return orders
        //        .Select(MapToResponse)
        //        .ToList();
        //}

        // Get a sales order by ID 
        public async Task<SalesOrderResponse> GetByIdAsync(Guid id)
        {
            // Get the sales order with the specified ID, including its items
            var order = await _context.SalesOrders
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            // If the order is not found, throw a NotFoundException
            if (order is null)
            {
                throw new NotFoundException(
                    $"Sales order with id '{id}' was not found.");
            }

            return MapToResponse(order);
        }

        // Confirm a sales order
        public async Task<SalesOrderResponse> ConfirmAsync(Guid id)
        {
            // Start a database transaction to ensure atomicity
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get order by ID, including its items
                var order = await _context.SalesOrders
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                // Check if the order exists
                if (order is null)
                {
                    throw new NotFoundException($"Sales order with id '{id}' was not found.");
                }

                // Check if the order is in Draft status
                if (order.Status != SalesOrderStatus.Draft)
                {
                    throw new ConflictException("Sales order has already been processed.");
                }

                // Check inventory for each item in the order
                foreach (var item in order.Items)
                {
                    // Check if the product has inventory
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    // If inventory is null, throw a ConflictException
                    if (inventory is null)
                    {
                        throw new ConflictException($"Product with id '{item.ProductId}' has no inventory.");
                    }

                    // Check if there is sufficient stock for the product
                    if (inventory.Quantity < item.Quantity)
                    {
                        throw new ConflictException($"Insufficient stock for product '{item.ProductId}'.");
                    }
                }

                // If all checks pass, update inventory and record stock transactions
                foreach (var item in order.Items)
                {
                    // Update inventory
                    var inventory = await _context.Inventories
                        .FirstAsync(x => x.ProductId == item.ProductId);

                    // Decrease the inventory quantity by the ordered quantity
                    inventory.Quantity -= item.Quantity;

                    // Record stock transaction for the stock out
                    var stockTransaction = new StockTransaction
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Type = StockTransactionType.StockOut,
                        Quantity = item.Quantity,
                        ReferenceType = "SalesOrder",
                        ReferenceId = order.Id,
                        Note = $"Stock out from sales order {order.OrderNumber}",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.StockTransactions.Add(stockTransaction);
                }

                // Update order status to Confirmed and set the updated timestamp
                order.Status = SalesOrderStatus.Confirmed;
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

        // Cancel a sales order
        public async Task<SalesOrderResponse> CancelAsync(Guid id)
        {
            // Get the sales order with the specified ID, including its items
            var order = await _context.SalesOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            // If the order is not found, throw a NotFoundException
            if (order is null)
            {
                throw new NotFoundException($"Sales order with id '{id}' was not found.");
            }

            // Check if the order is in Draft status; if not, throw a ConflictException
            if (order.Status != SalesOrderStatus.Draft)
            {
                throw new ConflictException("Sales order has already been processed.");
            }

            // Update order status to Cancelled and set the updated timestamp
            order.Status = SalesOrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(order);
        }

        // Get paginated sales orders based on query parameters
        public async Task<PaginatedList<SalesOrderResponse>> GetPagedAsync(SalesOrderQueryRequest request)
        {
            var query = _context.SalesOrders
                .AsNoTracking()
                .Include(x => x.Items)
                .AsQueryable();

            // Filter by status
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            // Filter from date
            if (request.FromDate.HasValue)
            {
                query = query.Where(x =>x.OrderDate >= request.FromDate.Value);
            }

            // Filter to date
            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1); // Include the entire day of the ToDate

                query = query.Where(x => x.OrderDate < toDate);
            }

            // Count after filtering
            var totalItems = await query.CountAsync();

            // Pagination
            var orders = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            var responses = orders
                .Select(MapToResponse)
                .ToList();

            return new PaginatedList<SalesOrderResponse>
                (
                    responses,
                    request.Page,
                    request.Limit,
                    totalItems
                );
        }

        private static SalesOrderResponse MapToResponse(SalesOrder salesOrder)
        {
            return new SalesOrderResponse
            {
                Id = salesOrder.Id,
                OrderNumber = salesOrder.OrderNumber,
                CustomerId = salesOrder.CustomerId,
                OrderDate = salesOrder.OrderDate,
                Status = salesOrder.Status,
                TotalAmount = salesOrder.TotalAmount,
                CreatedAt = salesOrder.CreatedAt,
                UpdatedAt = salesOrder.UpdatedAt,

                Items = salesOrder.Items
                    .Select(item => new SalesOrderItemResponse
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    })
                    .ToList()
            };
        }

        private static string GenerateOrderNumber()
        {
            return $"SO-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}