using OrderFlow.Application.Common.Contracts;
using OrderFlow.Application.Customers.Contracts;
using OrderFlow.Application.Customers.Dtos;
using OrderFlow.Application.Customers.Exceptions;
using OrderFlow.Application.Customers.Mappings;
using OrderFlow.Application.Customers.Repositories;
using OrderFlow.Domain.Customers;

namespace OrderFlow.Application.Customers.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository ??
            throw new ArgumentNullException(nameof(customerRepository));

        _unitOfWork = unitOfWork ??
            throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IReadOnlyCollection<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(
            cancellationToken);

        return customers
            .Select(customer => customer.ToResponse())
            .ToArray();
    }

    public async Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            id,
            cancellationToken);

        return customer?.ToResponse();
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = new Customer(
            request.Name,
            request.Email);

        var emailAlreadyExists =
            await _customerRepository.ExistsByEmailAsync(
                customer.Email,
                cancellationToken);

        if (emailAlreadyExists)
        {
            throw new CustomerEmailAlreadyExistsException();
        }

        await _customerRepository.AddAsync(
            customer,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return customer.ToResponse();
    }
}