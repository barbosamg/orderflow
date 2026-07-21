using OrderFlow.Domain.Orders;

namespace OrderFlow.Tests.Domain.Orders;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldCreateItem_WithSnapshotAndSubtotal()
    {
        var productId = Guid.NewGuid();

        var item = new OrderItem(
            productId,
            "  Roasted   Coffee  ",
            19.995m,
            3);

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal("Roasted Coffee", item.ProductName);
        Assert.Equal(20.00m, item.UnitPrice);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(60.00m, item.Subtotal);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenProductIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new OrderItem(
                Guid.Empty,
                "Coffee",
                10m,
                1));

        Assert.Equal("productId", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenProductNameIsInvalid(
        string? productName)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new OrderItem(
                Guid.NewGuid(),
                productName!,
                10m,
                1));

        Assert.Equal("productName", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRoundedUnitPriceIsNotPositive()
    {
        decimal[] invalidPrices =
        [
            0m,
            -1m,
            0.004m
        ];

        foreach (var invalidPrice in invalidPrices)
        {
            var exception =
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => new OrderItem(
                        Guid.NewGuid(),
                        "Coffee",
                        invalidPrice,
                        1));

            Assert.Equal("unitPrice", exception.ParamName);
        }
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenQuantityIsNotPositive()
    {
        int[] invalidQuantities =
        [
            0,
            -1
        ];

        foreach (var invalidQuantity in invalidQuantities)
        {
            var exception =
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => new OrderItem(
                        Guid.NewGuid(),
                        "Coffee",
                        10m,
                        invalidQuantity));

            Assert.Equal("quantity", exception.ParamName);
        }
    }
}