using OrderFlow.Domain.Chats;

namespace OrderFlow.Tests.Domain.Chats;

public class ChatMessageTests
{
    [Fact]
    public void Constructor_ShouldCreateMessage_WithInitialState()
    {
        var orderId = Guid.NewGuid();
        var beforeCreation = DateTimeOffset.UtcNow;

        var message = new ChatMessage(
            orderId,
            "Customer",
            "Hello!");

        var afterCreation = DateTimeOffset.UtcNow;

        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.Equal(orderId, message.OrderId);
        Assert.Equal("Customer", message.Sender);
        Assert.Equal("Hello!", message.Text);
        Assert.InRange(
            message.SentAt,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Constructor_ShouldNormalizeSender_AndTrimText()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            "  Customer   Support  ",
            "  How can I help?  ");

        Assert.Equal("Customer Support", message.Sender);
        Assert.Equal("How can I help?", message.Text);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyOrderId()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new ChatMessage(
                Guid.Empty,
                "Customer",
                "Hello!"));

        Assert.Equal("orderId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldRejectEmptySender(string sender)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new ChatMessage(
                Guid.NewGuid(),
                sender,
                "Hello!"));

        Assert.Equal("sender", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldRejectEmptyText(string text)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new ChatMessage(
                Guid.NewGuid(),
                "Customer",
                text));

        Assert.Equal("text", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldAcceptText_WithExactlyOneThousandCharacters()
    {
        var text = new string('x', 1_000);

        var message = new ChatMessage(
            Guid.NewGuid(),
            "Customer",
            text);

        Assert.Equal(1_000, message.Text.Length);
        Assert.Equal(text, message.Text);
    }

    [Fact]
    public void Constructor_ShouldRejectText_AboveOneThousandCharacters()
    {
        var text = new string('x', 1_001);

        var exception = Assert.Throws<ArgumentException>(
            () => new ChatMessage(
                Guid.NewGuid(),
                "Customer",
                text));

        Assert.Equal("text", exception.ParamName);
    }
}