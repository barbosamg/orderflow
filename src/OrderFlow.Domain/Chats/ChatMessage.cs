namespace OrderFlow.Domain.Chats;

public class ChatMessage
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public string Sender { get; private set; } = string.Empty;

    public string Text { get; private set; } = string.Empty;

    public DateTimeOffset SentAt { get; private set; }

    protected ChatMessage()
    {
    }

    public ChatMessage(
        Guid orderId,
        string sender,
        string text)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "Order identifier is required.",
                nameof(orderId));
        }

        if (string.IsNullOrWhiteSpace(sender))
        {
            throw new ArgumentException(
                "Message sender is required.",
                nameof(sender));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Message text is required.",
                nameof(text));
        }

        var normalizedSender = string.Join(
            ' ',
            sender.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries));

        var normalizedText = text.Trim();

        if (normalizedText.Length > 1_000)
        {
            throw new ArgumentException(
                "Message text cannot exceed 1,000 characters.",
                nameof(text));
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        Sender = normalizedSender;
        Text = normalizedText;
        SentAt = DateTimeOffset.UtcNow;
    }
}