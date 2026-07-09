using MacroViva.Domain.Common;

namespace MacroViva.Domain.ValueObjects;

public sealed record Money
{
    private Money()
    {
        Currency = string.Empty;
    }

    public Money(decimal amount, string currency)
    {
        Amount = Guard.AgainstNegative(amount, nameof(amount));
        Currency = Guard.AgainstNullOrWhiteSpace(currency, nameof(currency)).ToUpperInvariant();

        if (Currency.Length != 3)
        {
            throw new DomainException("Currency must be a three-letter ISO code.");
        }
    }

    public decimal Amount { get; }

    public string Currency { get; }
}
