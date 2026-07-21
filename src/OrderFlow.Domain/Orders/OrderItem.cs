namespace OrderFlow.Domain.Orders;

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal Subtotal => UnitPrice * Quantity;

    protected OrderItem()
    {
    }

    public OrderItem(
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product identifier is required.",
                nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(productName));
        }

        var normalizedUnitPrice = decimal.Round(
            unitPrice,
            2,
            MidpointRounding.AwayFromZero);

        if (normalizedUnitPrice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Unit price must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = NormalizeProductName(productName);
        UnitPrice = normalizedUnitPrice;
        Quantity = quantity;
    }

    private static string NormalizeProductName(string productName)
    {
        return string.Join(
            ' ',
            productName.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries));
    }
}
