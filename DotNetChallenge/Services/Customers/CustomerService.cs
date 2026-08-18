using DotNetChallenge.Common.Helpers;
using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Customers;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Common;
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

        //public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        //{
        //    return await _context.Customers
        //        .AsNoTracking()
        //        .OrderBy(x => x.Name)
        //        .Select(x => new CustomerResponse
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            Email = x.Email,
        //            Phone = x.Phone,
        //            Address = x.Address,
        //            CreatedAt = x.CreatedAt,
        //            UpdatedAt = x.UpdatedAt
        //        })
        //        .ToListAsync();
        //}

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
                    throw new DuplicatePhoneException("Customer phone already exists.");
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
                    throw new DuplicatePhoneException("Customer phone already exists.");
                }
            }

            customer.Name = request.Name.Trim();
            customer.Email = StringHelper.Normalize(request.Email);
            customer.Phone = phone;
            customer.Address = StringHelper.Normalize(request.Address);
            customer.UpdatedAt = DateTime.UtcNow;

            // Save changes with error handling for unique constraints
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("The phone or email might already be in use.");
            }

            return MapToResponse(customer);
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer is null)
            {
                throw new NotFoundException($"Customer with id '{id}' was not found.");
            }

            var hasOrders = await _context.SalesOrders
                .AnyAsync(x => x.CustomerId == id);

            if (hasOrders)
            {
                throw new ConflictException("Cannot delete customer because it has existing orders.");
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
        }

        // Paginated list of customers with search and pagination
        public async Task<PaginatedList<CustomerResponse>> GetPagedAsync(CustomerQueryRequest request)
        {
            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                // Case-insensitive search
                query = query.Where(x =>
                    EF.Functions.ILike(x.Name, $"%{search}%") || 
                    EF.Functions.ILike(x.Email, $"%{search}%") ||
                    EF.Functions.ILike(x.Phone, $"%{search}%"));
            }

            // Count after search
            var totalItems = await query.CountAsync();

            // Pagination
            var customers = await query
                .OrderBy(x => x.Name)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            var responses = customers
                .Select(MapToResponse)
                .ToList();

            return new PaginatedList<CustomerResponse>(
                responses,
                request.Page,
                request.Limit,
                totalItems);
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