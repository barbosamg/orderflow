namespace OrderFlow.Application.Products.Dtos;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price);