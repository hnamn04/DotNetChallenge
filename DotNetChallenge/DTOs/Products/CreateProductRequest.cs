namespace DotNetChallenge.DTOs.Products
{
    public class CreateProductRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UnitId { get; set; }
    }
}
