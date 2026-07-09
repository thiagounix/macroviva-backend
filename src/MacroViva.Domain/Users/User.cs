using MacroViva.Domain.Common;
using MacroViva.Domain.ConsentAndPrivacy;
using MacroViva.Domain.Enums;

namespace MacroViva.Domain.Users;

public sealed class User : AggregateRoot
{
    private readonly List<UserConsent> _consents = [];

    private User()
    {
        Email = string.Empty;
    }

    private User(Guid id, string email, LocaleCode locale, DateTimeOffset createdAt)
        : base(id)
    {
        Email = Guard.AgainstNullOrWhiteSpace(email, nameof(email));
        Locale = locale;
        CreatedAt = createdAt;
    }

    public string Email { get; }

    public LocaleCode Locale { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public UserProfile? Profile { get; private set; }

    public UserGoal? Goal { get; private set; }

    public IReadOnlyCollection<UserConsent> Consents => _consents.AsReadOnly();

    public static User Create(Guid id, string email, LocaleCode locale, DateTimeOffset createdAt)
    {
        return new User(id, email, locale, createdAt);
    }

    public void ChangeLocale(LocaleCode locale)
    {
        Locale = locale;
    }

    public void UpdateProfile(UserProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        EnsureBelongsToUser(profile.UserId);

        Profile = profile;
    }

    public void SetGoal(UserGoal goal)
    {
        ArgumentNullException.ThrowIfNull(goal);
        EnsureBelongsToUser(goal.UserId);

        Goal = goal;
    }

    public void RecordConsent(UserConsent consent)
    {
        ArgumentNullException.ThrowIfNull(consent);
        EnsureBelongsToUser(consent.UserId);

        _consents.Add(consent);
    }

    private void EnsureBelongsToUser(Guid userId)
    {
        if (userId != Id)
        {
            throw new DomainException("The item does not belong to this user.");
        }
    }
}
