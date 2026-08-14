using DotNetChallenge.DTOs.Customers;
using DotNetChallenge.Models.Common;
using DotNetChallenge.Services.Customers;
using Microsoft.AspNetCore.Mvc;

namespace DotNetChallenge.Controllers
{
    /// <summary>
    /// Customer management API.
    /// </summary>
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: /api/customers
        /// <summary>
        /// Gets a paginated list of customers.
        /// </summary>
        /// <returns> A paginated list of customers. </returns>
        /// <response code="200">Returns the paginated list of customers.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedList<CustomerResponse>>>> GetCustomers([FromQuery] CustomerQueryRequest request)
        {
            var result = await _customerService
                .GetPagedAsync(request);

            return Ok(new ApiResponse<PaginatedList<CustomerResponse>>(true, "Customers retrieved successfully.", result));
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
            var customer = await _customerService.GetByIdAsync(id);

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
            var customer = await _customerService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                customer);
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
            var customer = await _customerService.UpdateAsync(
                id,
                request);

            if (customer is null)
            {
                return NotFound(new
                {
                    message = $"Customer with id '{id}' was not found."
                });
            }

            return Ok(customer);
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
            await _customerService.DeleteAsync(id);

            return NoContent();
        }
    }
}