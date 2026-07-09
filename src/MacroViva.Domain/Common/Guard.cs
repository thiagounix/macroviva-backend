namespace MacroViva.Domain.Common;

public static class Guard
{
    public static Guid AgainstEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{parameterName} cannot be empty.");
        }

        return value;
    }

    public static string AgainstNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{parameterName} cannot be empty.");
        }

        return value.Trim();
    }

    public static decimal AgainstNegative(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new DomainException($"{parameterName} cannot be negative.");
        }

        return value;
    }

    public static decimal AgainstZeroOrNegative(decimal value, string parameterName)
    {
        if (value <= 0)
        {
            throw new DomainException($"{parameterName} must be greater than zero.");
        }

        return value;
    }

    public static DateTimeOffset AgainstDefault(DateTimeOffset value, string parameterName)
    {
        if (value == default)
        {
            throw new DomainException($"{parameterName} must be provided.");
        }

        return value;
    }
}
