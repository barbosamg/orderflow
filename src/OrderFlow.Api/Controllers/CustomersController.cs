using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Customers.Contracts;
using OrderFlow.Application.Customers.Dtos;
using OrderFlow.Application.Customers.Exceptions;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService ??
            throw new ArgumentNullException(nameof(customerService));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CustomerResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(
            cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                customer);
        }
        catch (CustomerEmailAlreadyExistsException exception)
        {
            return Conflict(
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Customer email already exists.",
                    Detail = exception.Message
                });
        }
    }
}