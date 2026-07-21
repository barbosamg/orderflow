using OrderFlow.Domain.Events;

namespace OrderFlow.Tests.Domain.Events;

public class ProcessedEventTests
{
    [Fact]
    public void Constructor_ShouldCreateProcessedEvent_WithInitialState()
    {
        var eventId = Guid.NewGuid();
        var beforeCreation = DateTimeOffset.UtcNow;

        var processedEvent = new ProcessedEvent(
            eventId,
            "order.created",
            """{"orderId":"123"}""");

        var afterCreation = DateTimeOffset.UtcNow;

        Assert.Equal(eventId, processedEvent.EventId);
        Assert.Equal("order.created", processedEvent.EventType);
        Assert.Equal(
            """{"orderId":"123"}""",
            processedEvent.Payload);
        Assert.InRange(
            processedEvent.ProcessedAt,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Constructor_ShouldTrimEventType_AndPayload()
    {
        var processedEvent = new ProcessedEvent(
            Guid.NewGuid(),
            "  order.created  ",
            """  {"orderId":"123"}  """);

        Assert.Equal("order.created", processedEvent.EventType);
        Assert.Equal(
            """{"orderId":"123"}""",
            processedEvent.Payload);
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyEventId()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new ProcessedEvent(
                Guid.Empty,
                "order.created",
                """{"orderId":"123"}"""));

        Assert.Equal("eventId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldRejectEmptyEventType(string eventType)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new ProcessedEvent(
                Guid.NewGuid(),
                eventType,
                """{"orderId":"123"}"""));

        Assert.Equal("eventType", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldRejectEmptyPayload(string payload)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new ProcessedEvent(
                Guid.NewGuid(),
                "order.created",
                payload));

        Assert.Equal("payload", exception.ParamName);
    }
}