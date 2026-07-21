using System.Net.Mail;

namespace OrderFlow.Domain.Customers;

public class Customer
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    protected Customer()
    {
    }

    public Customer(
        string name,
        string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Customer name is required.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Customer email is required.",
                nameof(email));
        }

        var normalizedName = string.Join(
            ' ',
            name.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries));

        var normalizedEmail = email
            .Trim()
            .ToLowerInvariant();

        if (!IsValidEmail(normalizedEmail))
        {
            throw new ArgumentException(
                "Customer email format is invalid.",
                nameof(email));
        }

        Id = Guid.NewGuid();
        Name = normalizedName;
        Email = normalizedEmail;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var parsedEmail = new MailAddress(email);

            return string.Equals(
                    parsedEmail.Address,
                    email,
                    StringComparison.Ordinal) &&
                parsedEmail.Host.Contains('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }
}