using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Payments;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Entities;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        // Create payment
        public async Task<PaymentResponse> CreateAsync(Guid salesOrderId, CreatePaymentRequest request)
        {
            // Check if the sales order exists
            var order = await _context.SalesOrders
                .FirstOrDefaultAsync(x => x.Id == salesOrderId);

            // If the order does not exist, throw a NotFoundException
            if (order is null)
            {
                throw new NotFoundException($"Sales order with id '{salesOrderId}' was not found.");
            }

            // Only confirmed order can receive payment
            if (order.Status != SalesOrderStatus.Confirmed)
            {
                throw new ConflictException("Payment can only be created for a confirmed sales order.");
            }

            // Calculate already paid amount
            var paidAmount = await _context.Payments
                .Where(x => x.SalesOrderId == salesOrderId)
                .SumAsync(x => x.Amount);

            // Check total payment
            var newTotal = paidAmount + request.Amount;

            // If the new total payment exceeds the order total amount, throw a ConflictException
            if (newTotal > order.TotalAmount)
            {
                throw new ConflictException("Total payment cannot exceed sales order total amount.");
            }

            // Determine payment status based on the new total payment
            var status = newTotal switch
            {
                0 => PaymentStatus.Unpaid,

                var amount when amount < order.TotalAmount => PaymentStatus.Partial,

                _ => PaymentStatus.Paid
            };

            // Create payment
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                SalesOrderId = salesOrderId,
                Amount = request.Amount,
                Method = request.Method,
                Status = status,
                PaidAt = status == PaymentStatus.Paid
                    ? DateTime.UtcNow
                    : null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return MapToResponse(payment);
        }

        // Get payments by sales order id
        public async Task<List<PaymentResponse>> GetBySalesOrderIdAsync(Guid salesOrderId)
        {
            // Check if the sales order exists
            var orderExists = await _context.SalesOrders
                .AnyAsync(x => x.Id == salesOrderId);

            // If the order does not exist, throw a NotFoundException
            if (!orderExists)
            {
                throw new NotFoundException($"Sales order with id '{salesOrderId}' was not found.");
            }

            // Get payments for the sales order
            var payments = await _context.Payments
                .AsNoTracking()
                .Where(x => x.SalesOrderId == salesOrderId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return payments
                .Select(MapToResponse)
                .ToList();
        }

        private static PaymentResponse MapToResponse(Payment payment)
        {
            return new PaymentResponse
            {
                Id = payment.Id,
                SalesOrderId = payment.SalesOrderId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
        }
    }
}
