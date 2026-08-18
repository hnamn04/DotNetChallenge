using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Suppliers;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;
using DotNetChallenge.Common.Helpers;

namespace DotNetChallenge.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;

        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        // Get all suppliers
        public async Task<IEnumerable<SupplierResponse>> GetAllAsync()
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SupplierResponse
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

        // Get supplier by id
        public async Task<SupplierResponse?> GetByIdAsync(Guid id)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SupplierResponse
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

        // Create a new supplier
        public async Task<SupplierResponse> CreateAsync(CreateSupplierRequest request)
        {
            var phone = StringHelper.NormalizePhone(request.Phone);

            if (phone is not null)
            {
                var phoneExists = await _context.Suppliers
                    .AnyAsync(x => x.Phone == phone);

                if (phoneExists)
                {
                    throw new DuplicatePhoneException("Supplier phone already exists.");
                }
            }

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = StringHelper.Normalize(request.Email),
                Phone = phone,
                Address = StringHelper.Normalize(request.Address),
                CreatedAt = DateTime.UtcNow
            };

            _context.Suppliers.Add(supplier);

            await _context.SaveChangesAsync();

            return MapToResponse(supplier);
        }

        // Update an existing supplier
        public async Task<SupplierResponse?> UpdateAsync(Guid id, UpdateSupplierRequest request)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (supplier is null)
            {
                return null;
            }

            var phone = StringHelper.NormalizePhone(request.Phone);

            if (phone is not null)
            {
                var phoneExists = await _context.Suppliers
                    .AnyAsync(x => x.Phone == phone && x.Id != id);

                if (phoneExists)
                {
                    throw new DuplicatePhoneException("Supplier phone already exists.");
                }
            }

            supplier.Name = request.Name.Trim();
            supplier.Email = StringHelper.Normalize(request.Email);
            supplier.Phone = phone;
            supplier.Address = StringHelper.Normalize(request.Address);
            supplier.UpdatedAt = DateTime.UtcNow;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) // Catch database update exceptions, which may occur due to unique constraint violations
            {
                throw new ConflictException("The phone or email might already be in use.");
            }

            return MapToResponse(supplier);
        }

        public async Task DeleteAsync(Guid id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (supplier is null)
            {
                throw new NotFoundException($"Supplier with id '{id}' was not found.");
            }

            var hasOrders = await _context.PurchaseOrders
                .AnyAsync(x => x.SupplierId == id);

            if (hasOrders)
            {
                throw new ConflictException("Cannot delete supplier because it has existing orders.");
            }

            _context.Suppliers.Remove(supplier);

            await _context.SaveChangesAsync();
        }

        private static SupplierResponse MapToResponse(Supplier supplier)
        {
            return new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }
    }
}