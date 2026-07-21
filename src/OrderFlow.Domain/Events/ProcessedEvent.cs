namespace OrderFlow.Domain.Events;

public class ProcessedEvent
{
    public Guid EventId { get; private set; }

    public string EventType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset ProcessedAt { get; private set; }

    protected ProcessedEvent()
    {
    }

    public ProcessedEvent(
        Guid eventId,
        string eventType,
        string payload)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event identifier is required.",
                nameof(eventId));
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException(
                "Event type is required.",
                nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException(
                "Event payload is required.",
                nameof(payload));
        }

        EventId = eventId;
        EventType = eventType.Trim();
        Payload = payload.Trim();
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}