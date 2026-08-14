using DotNetChallenge.DTOs.Customers;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.Suppliers;

namespace DotNetChallenge.Services.Customers
{
    public interface ICustomerService
    {
        //Task<IEnumerable<CustomerResponse>> GetAllAsync();

        Task<CustomerResponse?> GetByIdAsync(Guid id);

        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

        Task<CustomerResponse?> UpdateAsync(Guid id, UpdateCustomerRequest request);

        Task<PaginatedList<CustomerResponse>> GetPagedAsync(CustomerQueryRequest request);

        Task DeleteAsync(Guid id);
    }
}
