using OrderFlow.Application.Products.Dtos;

namespace OrderFlow.Application.Products.Contracts;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken);
}