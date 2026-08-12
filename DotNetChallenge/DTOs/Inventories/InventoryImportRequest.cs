namespace DotNetChallenge.DTOs.Inventories
{
    public class InventoryImportRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
