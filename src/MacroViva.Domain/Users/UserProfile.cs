using MacroViva.Domain.Common;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Users;

public sealed class UserProfile : Entity
{
    private UserProfile()
    {
        DisplayName = null!;
    }

    private UserProfile(Guid id, Guid userId, LocalizedName displayName, BodyMetrics? bodyMetrics)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        BodyMetrics = bodyMetrics;
    }

    public Guid UserId { get; }

    public LocalizedName DisplayName { get; private set; }

    public BodyMetrics? BodyMetrics { get; private set; }

    public static UserProfile Create(Guid id, Guid userId, LocalizedName displayName, BodyMetrics? bodyMetrics = null)
    {
        return new UserProfile(id, userId, displayName, bodyMetrics);
    }

    public void UpdateBodyMetrics(BodyMetrics bodyMetrics)
    {
        BodyMetrics = bodyMetrics ?? throw new ArgumentNullException(nameof(bodyMetrics));
    }
}
