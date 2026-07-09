using MacroViva.Domain.Nutrition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("Foods");
        builder.HasKey(food => food.Id);

        builder.OwnsOne(food => food.Name, name =>
        {
            name.Property(value => value.Value)
                .HasColumnName("Name")
                .HasMaxLength(180)
                .IsRequired();

            name.Property(value => value.Locale)
                .HasColumnName("NameLocale")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(food => food.Category)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(food => food.IsSupplement)
            .IsRequired();

        builder.OwnsOne(food => food.NutritionPer100g, nutrition =>
        {
            nutrition.OwnsOne(value => value.Macronutrients, macros =>
            {
                macros.Property(value => value.Calories).HasColumnName("CaloriesPer100g").HasPrecision(10, 2);
                macros.Property(value => value.ProteinGrams).HasColumnName("ProteinGramsPer100g").HasPrecision(10, 2);
                macros.Property(value => value.CarbohydrateGrams).HasColumnName("CarbohydrateGramsPer100g").HasPrecision(10, 2);
                macros.Property(value => value.FatGrams).HasColumnName("FatGramsPer100g").HasPrecision(10, 2);
            });
        });

        builder.HasMany(food => food.Portions)
            .WithOne()
            .HasForeignKey(portion => portion.FoodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(food => food.Portions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
