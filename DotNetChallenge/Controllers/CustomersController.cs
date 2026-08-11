using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Customers;
using DotNetChallenge.Models;
using DotNetChallenge.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Controllers
{
    /// <summary>
    /// Customer management API.
    /// </summary>
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/customers
        /// <summary>
        /// Gets all customers.
        /// </summary>
        /// <returns>A list of customers.</returns>
        /// <response code="200">Returns the customer list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetCustomers()
        {
            var customers = await _context.Customers
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

            return Ok(customers);
        }

        // GET: /api/customers/{id}
        /// <summary>
        /// Gets a customer by ID.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <response code="200">Returns the customer.</response>
        /// <response code="404">Customer was not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerResponse>> GetCustomerById(Guid id)
        {
            var customer = await _context.Customers
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

            if (customer is null)
            {
                return NotFound(new
                {
                    message = $"Customer with id '{id}' was not found."
                });
            }

            return Ok(customer);
        }

        // POST: /api/customers
        /// <summary>
        /// Creates a new customer.
        /// </summary>
        /// <param name="request">Customer information.</param>
        /// <response code="201">Customer was created successfully.</response>
        /// <response code="400">Request validation failed.</response>
        /// <response code="409">Customer phone already exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CustomerResponse>> CreateCustomer(CreateCustomerRequest request)
        {
            var normalizedPhone = request.Phone?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedPhone))
            {
                var phoneExists = await _context.Customers
                    .AnyAsync(x => x.Phone == normalizedPhone);

                if (phoneExists)
                {
                    return Conflict(new
                    {
                        message = "Customer phone already exists."
                    });
                }
            }

            var customer = new Customer
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

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                response);
        }

        // PUT: /api/customers/{id}
        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <param name="request">Updated customer information.</param>
        /// <response code="200">Customer was updated successfully.</response>
        /// <response code="400">Request validation failed.</response>
        /// <response code="404">Customer was not found.</response>
        /// <response code="409">Customer phone already exists.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer(Guid id, UpdateCustomerRequest request)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer is null)
            {
                return NotFound(new
                {
                    message = $"Customer with id '{id}' was not found."
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
                        message = "Customer phone already exists."
                    });
                }
            }

            customer.Name = request.Name.Trim();

            customer.Email = string.IsNullOrWhiteSpace(request.Email)
                ? null
                : request.Email.Trim();

            customer.Phone = normalizedPhone;

            customer.Address = string.IsNullOrWhiteSpace(request.Address)
                ? null
                : request.Address.Trim();

            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            });
        }

        // DELETE: /api/customers/{id}
        /// <summary>
        /// Deletes a customer.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <response code="204">Customer was deleted successfully.</response>
        /// <response code="404">Customer was not found.</response>
        /// <response code="409">Customer cannot be deleted because sales orders exist.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer is null)
            {
                return NotFound(new
                {
                    message = $"Customer with id '{id}' was not found."
                });
            }

            var hasOrders = await _context.SalesOrders
                .AnyAsync(x => x.CustomerId == id);

            if (hasOrders)
            {
                return Conflict(new
                {
                    message = "Cannot delete customer because the customer already has sales orders."
                });
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}