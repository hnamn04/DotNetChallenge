namespace DotNetChallenge.DTOs.Payments
{
    public class CreatePaymentRequest
    {
        public decimal Amount { get; set; }
        public string Method { get; set; } = null!;
    }
}
