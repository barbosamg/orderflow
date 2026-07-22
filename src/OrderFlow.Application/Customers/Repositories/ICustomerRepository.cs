using OrderFlow.Domain.Customers;

namespace OrderFlow.Application.Customers.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyCollection<Customer>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken);
}