namespace OrderFlow.Application.Products.Dtos;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock);