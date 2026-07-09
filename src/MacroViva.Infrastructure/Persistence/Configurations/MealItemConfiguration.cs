using MacroViva.Domain.Meals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class MealItemConfiguration : IEntityTypeConfiguration<MealItem>
{
    public void Configure(EntityTypeBuilder<MealItem> builder)
    {
        builder.ToTable("MealItems");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.MealId)
            .IsRequired();

        builder.Property(item => item.FoodId)
            .IsRequired();

        builder.OwnsOne(item => item.FoodNameSnapshot, name =>
        {
            name.Property(value => value.Value).HasColumnName("FoodNameSnapshot").HasMaxLength(180).IsRequired();
            name.Property(value => value.Locale).HasColumnName("FoodNameSnapshotLocale").HasConversion<string>().HasMaxLength(20).IsRequired();
        });

        builder.OwnsOne(item => item.Portion, portion =>
        {
            portion.Property(value => value.Quantity).HasColumnName("PortionQuantity").HasPrecision(10, 2);
            portion.Property(value => value.Unit).HasColumnName("PortionUnit").HasMaxLength(30).IsRequired();
            portion.Property(value => value.Grams).HasColumnName("PortionGrams").HasPrecision(10, 2);
        });

        builder.OwnsOne(item => item.MacronutrientsSnapshot, macros =>
        {
            macros.Property(value => value.Calories).HasColumnName("CaloriesSnapshot").HasPrecision(10, 2);
            macros.Property(value => value.ProteinGrams).HasColumnName("ProteinGramsSnapshot").HasPrecision(10, 2);
            macros.Property(value => value.CarbohydrateGrams).HasColumnName("CarbohydrateGramsSnapshot").HasPrecision(10, 2);
            macros.Property(value => value.FatGrams).HasColumnName("FatGramsSnapshot").HasPrecision(10, 2);
        });
    }
}
