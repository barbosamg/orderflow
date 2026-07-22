using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Customers.Repositories;
using OrderFlow.Domain.Customers;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Customers.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public CustomerRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext ??
            throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Name)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .AnyAsync(
                customer => customer.Email == email,
                cancellationToken);
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(customer);

        await _dbContext.Customers.AddAsync(
            customer,
            cancellationToken);
    }
}