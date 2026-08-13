using Microsoft.AspNetCore.Mvc;
using DotNetChallenge.DTOs.Inventories;
using DotNetChallenge.Services.Inventories;

namespace DotNetChallenge.Controllers
{
    [ApiController]
    [Route("api/inventories")]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("import")]
        public async Task<ActionResult<InventoryResponse>> Import(InventoryImportRequest request)
        {
            var response = await _inventoryService.ImportAsync(request);

            return Ok(response);
        }

        [HttpPost("export")]
        public async Task<ActionResult<InventoryResponse>> Export(InventoryExportRequest request)
        {
            var response = await _inventoryService.ExportAsync(request);

            return Ok(response);
        }

        [HttpGet("products/{productId:guid}")]
        public async Task<ActionResult<InventoryResponse>> GetByProductId(Guid productId)
        {
            var response = await _inventoryService
                .GetByProductIdAsync(productId);

            return Ok(response);
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<StockTransactionResponse>>>GetTransactions()
        {
            var response = await _inventoryService
                .GetTransactionsAsync();

            return Ok(response);
        }
    }
}
