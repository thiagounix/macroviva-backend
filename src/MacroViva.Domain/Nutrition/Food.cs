using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Nutrition;

public sealed class Food : AggregateRoot
{
    private readonly List<FoodPortion> _portions = [];

    private Food()
    {
        Name = null!;
        NutritionPer100g = null!;
    }

    private Food(
        Guid id,
        LocalizedName name,
        FoodCategory category,
        NutritionPer100g nutritionPer100g,
        bool isSupplement)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category;
        NutritionPer100g = nutritionPer100g ?? throw new ArgumentNullException(nameof(nutritionPer100g));
        IsSupplement = isSupplement;
    }

    public LocalizedName Name { get; private set; }

    public FoodCategory Category { get; private set; }

    public NutritionPer100g NutritionPer100g { get; private set; }

    public bool IsSupplement { get; }

    public IReadOnlyCollection<FoodPortion> Portions => _portions.AsReadOnly();

    public static Food Create(
        Guid id,
        LocalizedName name,
        FoodCategory category,
        NutritionPer100g nutritionPer100g,
        bool isSupplement = false)
    {
        return new Food(id, name, category, nutritionPer100g, isSupplement);
    }

    public Macronutrients CalculateFor(Portion portion)
    {
        return NutritionPer100g.CalculateFor(portion);
    }

    public void Rename(LocalizedName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateNutrition(NutritionPer100g nutritionPer100g)
    {
        NutritionPer100g = nutritionPer100g ?? throw new ArgumentNullException(nameof(nutritionPer100g));
    }

    public void AddPortion(FoodPortion portion)
    {
        ArgumentNullException.ThrowIfNull(portion);

        if (portion.FoodId != Id)
        {
            throw new DomainException("Food portion does not belong to this food.");
        }

        _portions.Add(portion);
    }
}
