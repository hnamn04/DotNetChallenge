namespace DotNetChallenge.DTOs.Customers
{
    /// <summary>
    /// Request data used to create a new customer.
    /// </summary>
    public class CreateCustomerRequest
    {
        /// <summary>
        /// Customer name. Required.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Customer email address. Optional.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Customer phone number. Must be unique among customers if provided.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Customer address. Optional.
        /// </summary>
        public string? Address { get; set; }
    }
}
