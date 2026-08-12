using DotNetChallenge.DTOs.Inventories;

namespace DotNetChallenge.Services.Inventories
{
    public interface IInventoryService
    {
        Task<InventoryResponse> ImportAsync(InventoryImportRequest request);

        Task<InventoryResponse> ExportAsync(InventoryExportRequest request);

        Task<InventoryResponse> GetByProductIdAsync(Guid productId);

        Task<IEnumerable<StockTransactionResponse>> GetTransactionsAsync();
    }
}
