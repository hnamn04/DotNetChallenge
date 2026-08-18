using DotNetChallenge.DTOs.Inventories;
using DotNetChallenge.Models.Common;

namespace DotNetChallenge.Services.Inventories
{
    public interface IInventoryService
    {
        Task<InventoryResponse> ImportAsync(InventoryImportRequest request);

        Task<InventoryResponse> ExportAsync(InventoryExportRequest request);

        Task<InventoryResponse> GetByProductIdAsync(Guid productId);

        Task<PaginatedList<StockTransactionResponse>> GetPagedTransactionsAsync(StockTransactionQueryRequest request);
    }
}
