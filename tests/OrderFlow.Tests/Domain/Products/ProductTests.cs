using OrderFlow.Domain.Products;

namespace OrderFlow.Tests.Domain.Products;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateActiveProduct_WithNormalizedValues()
    {
        var beforeCreation = DateTimeOffset.UtcNow;

        var product = new Product(
            "  Coffee  ",
            "  Roasted coffee  ",
            12.345m,
            10);

        var afterCreation = DateTimeOffset.UtcNow;

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Coffee", product.Name);
        Assert.Equal("Roasted coffee", product.Description);
        Assert.Equal(12.35m, product.Price);
        Assert.Equal(10, product.Stock);
        Assert.True(product.IsActive);
        Assert.InRange(
            product.CreatedAt,
            beforeCreation,
            afterCreation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldSetDescriptionToNull_WhenDescriptionIsEmpty(
        string? description)
    {
        var product = new Product(
            "Coffee",
            description,
            12.50m,
            10);

        Assert.Null(product.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(string? name)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Product(
                name!,
                "Description",
                10m,
                5));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenRoundedPriceIsNotPositive()
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
                    () => new Product(
                        "Coffee",
                        "Description",
                        invalidPrice,
                        5));

            Assert.Equal("price", exception.ParamName);
        }
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenStockIsNegative()
    {
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Product(
                    "Coffee",
                    "Description",
                    10m,
                    -1));

        Assert.Equal("stock", exception.ParamName);
    }

    [Fact]
    public void IncreaseStock_ShouldAddQuantityToCurrentStock()
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        product.IncreaseStock(5);

        Assert.Equal(15, product.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseStock_ShouldThrow_WhenQuantityIsInvalid(
        int quantity)
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => product.IncreaseStock(quantity));

        Assert.Equal("quantity", exception.ParamName);
        Assert.Equal(10, product.Stock);
    }

    [Fact]
    public void DecreaseStock_ShouldSubtractQuantityFromCurrentStock()
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        product.DecreaseStock(4);

        Assert.Equal(6, product.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DecreaseStock_ShouldThrow_WhenQuantityIsInvalid(
        int quantity)
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => product.DecreaseStock(quantity));

        Assert.Equal("quantity", exception.ParamName);
        Assert.Equal(10, product.Stock);
    }

    [Fact]
    public void DecreaseStock_ShouldThrow_WhenStockIsInsufficient()
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        Assert.Throws<InvalidOperationException>(
            () => product.DecreaseStock(11));

        Assert.Equal(10, product.Stock);
    }

    [Fact]
    public void Deactivate_ShouldSetProductAsInactive()
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        product.Deactivate();

        Assert.False(product.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetProductAsActive()
    {
        var product = new Product(
            "Coffee",
            null,
            10m,
            10);

        product.Deactivate();
        product.Activate();

        Assert.True(product.IsActive);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateAndNormalizeProductData()
    {
        var product = new Product(
            "Coffee",
            "Original description",
            10m,
            5);

        var originalId = product.Id;
        var originalCreatedAt = product.CreatedAt;

        product.UpdateDetails(
            "  Premium Coffee  ",
            "  New description  ",
            20.555m);

        Assert.Equal(originalId, product.Id);
        Assert.Equal("Premium Coffee", product.Name);
        Assert.Equal("New description", product.Description);
        Assert.Equal(20.56m, product.Price);
        Assert.Equal(5, product.Stock);
        Assert.True(product.IsActive);
        Assert.Equal(originalCreatedAt, product.CreatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ShouldSetDescriptionToNull_WhenDescriptionIsEmpty(
        string? description)
    {
        var product = new Product(
            "Coffee",
            "Original description",
            10m,
            5);

        product.UpdateDetails(
            "Coffee",
            description,
            20m);

        Assert.Null(product.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ShouldThrowWithoutChangingProduct_WhenNameIsInvalid(
        string? name)
    {
        var product = new Product(
            "Coffee",
            "Original description",
            10m,
            5);

        var exception = Assert.Throws<ArgumentException>(
            () => product.UpdateDetails(
                name!,
                "New description",
                20m));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Coffee", product.Name);
        Assert.Equal("Original description", product.Description);
        Assert.Equal(10m, product.Price);
    }

    [Fact]
    public void UpdateDetails_ShouldThrowWithoutChangingProduct_WhenPriceIsInvalid()
    {
        decimal[] invalidPrices =
        [
            0m,
            -1m,
            0.004m
        ];

        foreach (var invalidPrice in invalidPrices)
        {
            var product = new Product(
                "Coffee",
                "Original description",
                10m,
                5);

            var exception =
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => product.UpdateDetails(
                        "New coffee",
                        "New description",
                        invalidPrice));

            Assert.Equal("price", exception.ParamName);
            Assert.Equal("Coffee", product.Name);
            Assert.Equal(
                "Original description",
                product.Description);
            Assert.Equal(10m, product.Price);
        }
    }
}