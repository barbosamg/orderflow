using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Products.Repositories;
using OrderFlow.Domain.Products;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Products.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public ProductRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext ??
            throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .SingleOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);

        await _dbContext.Products.AddAsync(
            product,
            cancellationToken);
    }
}