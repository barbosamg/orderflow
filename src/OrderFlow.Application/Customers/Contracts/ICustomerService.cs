using OrderFlow.Application.Customers.Dtos;

namespace OrderFlow.Application.Customers.Contracts;

public interface ICustomerService
{
    Task<IReadOnlyCollection<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken);
}