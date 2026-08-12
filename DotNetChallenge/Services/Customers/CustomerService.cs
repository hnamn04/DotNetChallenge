using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Customers;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Common.Helpers;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new CustomerResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Address = x.Address,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<CustomerResponse?> GetByIdAsync(Guid id)
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomerResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Address = x.Address,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            var phone = StringHelper.NormalizePhone(request.Phone);

            if (phone is not null)
            {
                var phoneExists = await _context.Customers
                    .AnyAsync(x => x.Phone == phone);

                if (phoneExists)
                {
                    throw new DuplicatePhoneException(
                        "Customer phone already exists.");
                }
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = StringHelper.Normalize(request.Email),
                Phone = phone,
                Address = StringHelper.Normalize(request.Address),
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return MapToResponse(customer);
        }

        public async Task<CustomerResponse?> UpdateAsync(Guid id, UpdateCustomerRequest request)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer is null)
            {
                return null;
            }

            var phone = StringHelper.NormalizePhone(request.Phone);

            if (phone is not null)
            {
                var phoneExists = await _context.Customers
                    .AnyAsync(x =>
                        x.Phone == phone &&
                        x.Id != id);

                if (phoneExists)
                {
                    throw new DuplicatePhoneException(
                        "Customer phone already exists.");
                }
            }

            customer.Name = request.Name.Trim();
            customer.Email = StringHelper.Normalize(request.Email);
            customer.Phone = phone;
            customer.Address = StringHelper.Normalize(request.Address);
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(customer);
        }

        public async Task<CustomerDeleteResult> DeleteAsync(Guid id)
        {
            var customer = await _context.Customers
                .Include(x => x.SalesOrders)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer is null)
            {
                return CustomerDeleteResult.NotFound;
            }

            if (customer.SalesOrders.Any())
            {
                return CustomerDeleteResult.HasOrders;
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return CustomerDeleteResult.Deleted;
        }

        public async Task<bool> PhoneExistsAsync(string phone, Guid? excludeId = null)
        {
            return await _context.Customers
                .AnyAsync(x =>
                    x.Phone == phone &&
                    (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        private static CustomerResponse MapToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }
    }
}