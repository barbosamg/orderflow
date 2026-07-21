using OrderFlow.Domain.Customers;

namespace OrderFlow.Tests.Domain.Customers;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldCreateCustomer_WithNormalizedValues()
    {
        var beforeCreation = DateTimeOffset.UtcNow;

        var customer = new Customer(
            "  Maria   da\tSilva  ",
            "  MARIA.SILVA@EXAMPLE.COM  ");

        var afterCreation = DateTimeOffset.UtcNow;

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Maria da Silva", customer.Name);
        Assert.Equal(
            "maria.silva@example.com",
            customer.Email);
        Assert.InRange(
            customer.CreatedAt,
            beforeCreation,
            afterCreation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(
        string? name)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Customer(
                name!,
                "maria@example.com"));

        Assert.Equal("name", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("maria@")]
    [InlineData("@example.com")]
    [InlineData("maria@example")]
    [InlineData("Maria <maria@example.com>")]
    public void Constructor_ShouldThrow_WhenEmailIsInvalid(
        string? email)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Customer(
                "Maria da Silva",
                email!));

        Assert.Equal("email", exception.ParamName);
    }
}
