namespace DotNetChallenge.DTOs.Suppliers
{
    /// <summary>
    /// Response data representing a supplier.
    /// </summary>
    public class SupplierResponse
    {
        /// <summary>
        /// Supplier unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Supplier name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Supplier email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Supplier phone number.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Supplier address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Date and time when the supplier was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the supplier was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
