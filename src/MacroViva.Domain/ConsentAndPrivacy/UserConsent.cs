using MacroViva.Domain.Common;

namespace MacroViva.Domain.ConsentAndPrivacy;

public sealed class UserConsent : Entity
{
    private UserConsent()
    {
        ConsentKey = string.Empty;
    }

    private UserConsent(Guid id, Guid userId, string consentKey, bool isGranted, DateTimeOffset decidedAt)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        ConsentKey = Guard.AgainstNullOrWhiteSpace(consentKey, nameof(consentKey));
        IsGranted = isGranted;
        DecidedAt = decidedAt;
    }

    public Guid UserId { get; }

    public string ConsentKey { get; }

    public bool IsGranted { get; }

    public DateTimeOffset DecidedAt { get; }

    public static UserConsent Grant(Guid id, Guid userId, string consentKey, DateTimeOffset decidedAt)
    {
        return new UserConsent(id, userId, consentKey, isGranted: true, decidedAt);
    }

    public static UserConsent Revoke(Guid id, Guid userId, string consentKey, DateTimeOffset decidedAt)
    {
        return new UserConsent(id, userId, consentKey, isGranted: false, decidedAt);
    }
}
