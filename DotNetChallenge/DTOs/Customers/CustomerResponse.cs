namespace DotNetChallenge.DTOs.Customers
{
    /// <summary>
    /// Response data representing a customer.
    /// </summary>
    public class CustomerResponse
    {
        /// <summary>
        /// Customer unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Customer name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Customer email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Customer phone number.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Customer address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Date and time when the customer was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the customer was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
