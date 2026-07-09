using MacroViva.Domain.Common;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Nutrition;

public sealed class FoodPortion : Entity
{
    private FoodPortion()
    {
        Name = null!;
        Portion = null!;
    }

    private FoodPortion(Guid id, Guid foodId, LocalizedName name, Portion portion)
        : base(id)
    {
        FoodId = Guard.AgainstEmpty(foodId, nameof(foodId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Portion = portion ?? throw new ArgumentNullException(nameof(portion));
    }

    public Guid FoodId { get; }

    public LocalizedName Name { get; }

    public Portion Portion { get; }

    public static FoodPortion Create(Guid id, Guid foodId, LocalizedName name, Portion portion)
    {
        return new FoodPortion(id, foodId, name, portion);
    }
}
