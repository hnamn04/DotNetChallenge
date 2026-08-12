using DotNetChallenge.DTOs.Suppliers;

namespace DotNetChallenge.Services.Suppliers
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponse>> GetAllAsync();

        Task<SupplierResponse?> GetByIdAsync(Guid id);

        Task<SupplierResponse> CreateAsync(CreateSupplierRequest request);

        Task<SupplierResponse?> UpdateAsync(Guid id, UpdateSupplierRequest request);

        Task DeleteAsync(Guid id);
    }
}
