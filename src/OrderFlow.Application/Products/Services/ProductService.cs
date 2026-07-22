using OrderFlow.Application.Common.Contracts;
using OrderFlow.Application.Products.Contracts;
using OrderFlow.Application.Products.Dtos;
using OrderFlow.Application.Products.Mappings;
using OrderFlow.Application.Products.Repositories;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Products.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository ??
            throw new ArgumentNullException(nameof(productRepository));

        _unitOfWork = unitOfWork ??
            throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(
            cancellationToken);

        return products
            .Select(product => product.ToResponse())
            .ToArray();
    }

    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        return product?.ToResponse();
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return product.ToResponse();
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.UpdateDetails(
            request.Name,
            request.Description,
            request.Price);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return product.ToResponse();
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Deactivate();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}