using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;

namespace MacroViva.Domain.ValueObjects;

public sealed record LocalizedName
{
    private LocalizedName()
    {
        Value = string.Empty;
    }

    public LocalizedName(string value, LocaleCode locale)
    {
        Value = Guard.AgainstNullOrWhiteSpace(value, nameof(value));
        Locale = locale;
    }

    public string Value { get; }

    public LocaleCode Locale { get; }

    public override string ToString()
    {
        return Value;
    }
}
