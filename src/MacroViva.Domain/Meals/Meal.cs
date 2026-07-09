using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Meals;

public sealed class Meal : AggregateRoot
{
    private readonly List<MealItem> _items = [];
    private readonly List<MealPhoto> _photos = [];

    private Meal()
    {
        TotalMacronutrients = null!;
    }

    private Meal(Guid id, Guid userId, MealType mealType, DateTimeOffset occurredAt)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        MealType = mealType;
        OccurredAt = occurredAt;
        TotalMacronutrients = Macronutrients.Zero;
    }

    public Guid UserId { get; }

    public MealType MealType { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public Macronutrients TotalMacronutrients { get; private set; }

    public IReadOnlyCollection<MealItem> Items => _items.AsReadOnly();

    public IReadOnlyCollection<MealPhoto> Photos => _photos.AsReadOnly();

    public static Meal Create(Guid id, Guid userId, MealType mealType, DateTimeOffset occurredAt)
    {
        return new Meal(id, userId, mealType, occurredAt);
    }

    public MealItem AddFood(Guid mealItemId, Food food, Portion portion)
    {
        var item = MealItem.FromFood(mealItemId, Id, food, portion);

        _items.Add(item);
        RecalculateTotal();

        return item;
    }

    public void RemoveItem(Guid mealItemId)
    {
        var removed = _items.RemoveAll(item => item.Id == mealItemId);

        if (removed == 0)
        {
            throw new DomainException("Meal item was not found.");
        }

        RecalculateTotal();
    }

    public void AddPhoto(MealPhoto photo)
    {
        ArgumentNullException.ThrowIfNull(photo);

        if (photo.MealId != Id)
        {
            throw new DomainException("Meal photo does not belong to this meal.");
        }

        _photos.Add(photo);
    }

    private void RecalculateTotal()
    {
        TotalMacronutrients = _items.Aggregate(
            Macronutrients.Zero,
            (total, item) => total + item.MacronutrientsSnapshot);
    }
}
