using OrderFlow.Application.Products.Dtos;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Products.Mappings;

internal static class ProductMappings
{
    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.CreatedAt);
    }
}