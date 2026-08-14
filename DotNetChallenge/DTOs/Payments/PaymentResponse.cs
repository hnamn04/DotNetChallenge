using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.DTOs.Payments
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid SalesOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = null!;
        public PaymentStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
