using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Suppliers;
using DotNetChallenge.Models;
using DotNetChallenge.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Controllers
{
    /// <summary>
    /// Supplier management API.
    /// </summary>
    [ApiController]
    [Route("api/suppliers")]
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public  SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/suppliers
        /// <summary>
        /// Gets all suppliers.
        /// </summary>
        /// <returns>A list of suppliers.</returns>
        /// <response code="200">Returns the supplier list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SupplierResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierResponse>>> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
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

            return Ok(suppliers);
        }

        // GET: /api/supplier/{id}
        /// <summary>
        /// Gets a supplier by ID.
        /// </summary>
        /// <param name="id">Supplier unique identifier.</param>
        /// <response code="200">Returns the supplier.</response>
        /// <response code="404">Supplier was not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierResponse>> GetSuppliersById(Guid id)
        {
            var supplier = await _context.Suppliers
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

            if (supplier is null)
            {
                return NotFound(new
                {
                    message = $"Supplier with id '{id}' was not found."
                });
            }

            return Ok(supplier);
        }

        // POST: /api/supplier
        /// <summary>
        /// Creates a new supplier.
        /// </summary>
        /// <param name="request">Supplier creation data.</param>
        /// <response code="201">Supplier was created successfully.</response>
        /// <response code="400">Request validation failed.</response>
        /// <response code="409">Supplier phone already exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SupplierResponse>> CreateSupplier(CreateSupplierRequest request)
        {
            var normalizedPhone = request.Phone?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedPhone))
            {
                var phoneExists = await _context.Suppliers
                    .AnyAsync(x => x.Phone == normalizedPhone);

                if (phoneExists)
                {
                    return Conflict(new
                    {
                        message = "Supplier phone already exists."
                    });
                }
            }

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email)
                    ? null
                    : request.Email.Trim(),
                Phone = normalizedPhone,
                Address = string.IsNullOrWhiteSpace(request.Address)
                    ? null
                    : request.Address.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _context.Suppliers.Add(supplier);

            await _context.SaveChangesAsync();

            var response = new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };

            return CreatedAtAction(
                nameof(GetSuppliersById),
                new { id = supplier.Id },
                response);
        }

        // PUT: /api/supplier/{id}
        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        /// <param name="id">Supplier unique identifier.</param>
        /// <param name="request">Supplier update data.</param>
        /// <response code="200">Supplier was updated successfully.</response>
        /// <response code="400">Request validation failed.</response>
        /// <response code="404">Supplier was not found.</response>
        /// <response code="409">Supplier phone already exists.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SupplierResponse>> UpdateSupplier(Guid id, UpdateSupplierRequest request)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (supplier is null)
            {
                return NotFound(new
                {
                    message = $"Supplier with id '{id}' was not found."
                });
            }

            var normalizedPhone = request.Phone?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedPhone))
            {
                var phoneExists = await _context.Customers
                    .AnyAsync(x =>
                        x.Phone == normalizedPhone &&
                        x.Id != id);

                if (phoneExists)
                {
                    return Conflict(new
                    {
                        message = "Supplier phone already exists."
                    });
                }
            }

            supplier.Name = request.Name.Trim();

            supplier.Email = string.IsNullOrWhiteSpace(request.Email)
                ? null
                : request.Email.Trim();

            supplier.Phone = normalizedPhone;

            supplier.Address = string.IsNullOrWhiteSpace(request.Address)
                ? null
                : request.Address.Trim();

            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            });
        }

        // DELETE: /api/suppliers/{id}
        /// <summary>
        /// Deletes a supplier.
        /// </summary>
        /// <param name="id">Supplier unique identifier.</param>
        /// <response code="204">Supplier was deleted successfully.</response>
        /// <response code="404">Supplier was not found.</response>
        /// <response code="409">
        /// Supplier cannot be deleted because purchase orders exist.
        /// </response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteSuppler(Guid id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (supplier is null)
            {
                return NotFound(new
                {
                    message = $"Supplier with id '{id}' was not found."
                });
            }

            var hasOrders = await _context.PurchaseOrders
                .AnyAsync(x => x.SupplierId == id);

            if (hasOrders)
            {
                return Conflict(new
                {
                    message = "Cannot delete supplier because the supplier already has purchase orders."
                });
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
