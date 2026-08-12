namespace DotNetChallenge.DTOs.Inventories
{
    public class InventoryResponse
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public int ReservedQuantity { get; set; }

        public int AvailableQuantity => Quantity - ReservedQuantity;
    }
}
