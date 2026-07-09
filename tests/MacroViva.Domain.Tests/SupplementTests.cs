using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.Supplements;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Tests;

public sealed class SupplementTests
{
    private static readonly DateOnly CheckInDate = new(2026, 1, 1);

    [Fact]
    public void ShouldAllowWheyProteinToAddProtein()
    {
        var whey = Supplement.CreateWheyProtein(
            Guid.NewGuid(),
            new LocalizedName("Whey protein", LocaleCode.EnUs),
            new Macronutrients(calories: 120m, proteinGrams: 24m, carbohydrateGrams: 3m, fatGrams: 2m));

        var checkIn = UserSupplement.CheckIn(
            Guid.NewGuid(),
            Guid.NewGuid(),
            whey,
            CheckInDate,
            servings: 2m);

        Assert.Equal(240m, checkIn.MacronutrientImpact.Calories);
        Assert.Equal(48m, checkIn.MacronutrientImpact.ProteinGrams);
        Assert.Equal(6m, checkIn.MacronutrientImpact.CarbohydrateGrams);
        Assert.Equal(4m, checkIn.MacronutrientImpact.FatGrams);
    }

    [Fact]
    public void ShouldAllowCreatineCheckInWithoutMacros()
    {
        var creatine = Supplement.CreateCreatine(
            Guid.NewGuid(),
            new LocalizedName("Creatine", LocaleCode.EnUs));

        var checkIn = UserSupplement.CheckIn(
            Guid.NewGuid(),
            Guid.NewGuid(),
            creatine,
            CheckInDate,
            servings: 1m);

        Assert.Equal(Macronutrients.Zero, checkIn.MacronutrientImpact);
    }

    [Fact]
    public void ShouldFailWhenWheyProteinNameIsEmpty()
    {
        Assert.Throws<DomainException>(() => Supplement.CreateWheyProtein(
            Guid.NewGuid(),
            new LocalizedName("", LocaleCode.EnUs),
            new Macronutrients(calories: 120m, proteinGrams: 24m, carbohydrateGrams: 3m, fatGrams: 2m)));
    }

    [Fact]
    public void ShouldFailWhenWheyProteinHasZeroProtein()
    {
        Assert.Throws<DomainException>(() => Supplement.CreateWheyProtein(
            Guid.NewGuid(),
            new LocalizedName("Whey protein", LocaleCode.EnUs),
            new Macronutrients(calories: 120m, proteinGrams: 0m, carbohydrateGrams: 3m, fatGrams: 2m)));
    }

    [Fact]
    public void ShouldFailWhenWheyProteinHasNegativeProtein()
    {
        Assert.Throws<DomainException>(() => Supplement.CreateWheyProtein(
            Guid.NewGuid(),
            new LocalizedName("Whey protein", LocaleCode.EnUs),
            new Macronutrients(calories: 120m, proteinGrams: -1m, carbohydrateGrams: 3m, fatGrams: 2m)));
    }
}
