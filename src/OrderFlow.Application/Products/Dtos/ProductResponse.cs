namespace OrderFlow.Application.Products.Dtos;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTimeOffset CreatedAt);