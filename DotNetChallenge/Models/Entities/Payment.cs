using DotNetChallenge.Models.Enums;

namespace DotNetChallenge.Models.Entities
{
    public class Payment : BaseEntity
    {
        public Guid SalesOrderId { get; set; }

        public SalesOrder SalesOrder { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Method { get; set; } = null!;

        public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;

        public DateTime? PaidAt { get; set; }
    }
}
