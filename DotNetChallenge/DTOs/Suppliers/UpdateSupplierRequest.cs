namespace DotNetChallenge.DTOs.Suppliers
{
    /// <summary>
    /// Request data used to update an existing supplier.
    /// </summary>
    public class UpdateSupplierRequest
    {
        /// <summary>
        /// Supplier name. Required.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Supplier email address. Optional.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Supplier phone number. Must be unique among suppliers if provided.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Supplier address. Optional.
        /// </summary>
        public string? Address { get; set; }
    }
}
