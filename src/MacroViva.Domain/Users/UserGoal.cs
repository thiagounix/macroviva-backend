using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Users;

public sealed class UserGoal : Entity
{
    private UserGoal()
    {
        DailyTargets = null!;
    }

    private UserGoal(Guid id, Guid userId, UserObjective objective, ActivityLevel activityLevel, DailyTargets dailyTargets)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        Objective = objective;
        ActivityLevel = activityLevel;
        DailyTargets = dailyTargets ?? throw new ArgumentNullException(nameof(dailyTargets));
    }

    public Guid UserId { get; }

    public UserObjective Objective { get; private set; }

    public ActivityLevel ActivityLevel { get; private set; }

    public DailyTargets DailyTargets { get; private set; }

    public static UserGoal Create(
        Guid id,
        Guid userId,
        UserObjective objective,
        ActivityLevel activityLevel,
        DailyTargets dailyTargets)
    {
        return new UserGoal(id, userId, objective, activityLevel, dailyTargets);
    }

    public void UpdateTargets(DailyTargets dailyTargets)
    {
        DailyTargets = dailyTargets ?? throw new ArgumentNullException(nameof(dailyTargets));
    }
}
