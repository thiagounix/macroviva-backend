using MacroViva.Domain.Common;

namespace MacroViva.Domain.Meals;

public sealed class MealPhoto : Entity
{
    private MealPhoto(Guid id, Guid mealId, string temporaryReference, string contentType, DateTimeOffset capturedAt)
        : base(id)
    {
        MealId = Guard.AgainstEmpty(mealId, nameof(mealId));
        TemporaryReference = Guard.AgainstNullOrWhiteSpace(temporaryReference, nameof(temporaryReference));
        ContentType = Guard.AgainstNullOrWhiteSpace(contentType, nameof(contentType));
        CapturedAt = capturedAt;
    }

    public Guid MealId { get; }

    public string TemporaryReference { get; }

    public string ContentType { get; }

    public DateTimeOffset CapturedAt { get; }

    public static MealPhoto Create(
        Guid id,
        Guid mealId,
        string temporaryReference,
        string contentType,
        DateTimeOffset capturedAt)
    {
        return new MealPhoto(id, mealId, temporaryReference, contentType, capturedAt);
    }
}
