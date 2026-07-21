namespace OrderFlow.Domain.Products;

public class Product
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    protected Product()
    {
    }

    public Product(
    string name,
    string? description,
    decimal price,
    int stock)
    {
        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stock),
                "Product stock cannot be negative.");
        }

        Id = Guid.NewGuid();
        Stock = stock;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;

        UpdateDetails(name, description, price);
    }

    public void UpdateDetails(
    string name,
    string? description,
    decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        var normalizedPrice = decimal.Round(
            price,
            2,
            MidpointRounding.AwayFromZero);

        if (normalizedPrice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Product price must be greater than zero.");
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        Price = normalizedPrice;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        Stock = checked(Stock + quantity);
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        if (quantity > Stock)
        {
            throw new InvalidOperationException(
                "Insufficient product stock.");
        }

        Stock -= quantity;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}