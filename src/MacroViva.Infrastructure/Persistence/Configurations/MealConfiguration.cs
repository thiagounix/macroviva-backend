using MacroViva.Domain.Meals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meals");
        builder.HasKey(meal => meal.Id);

        builder.Property(meal => meal.UserId)
            .IsRequired();

        builder.Property(meal => meal.MealType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(meal => meal.OccurredAt)
            .IsRequired();

        builder.OwnsOne(meal => meal.TotalMacronutrients, macros =>
        {
            macros.Property(value => value.Calories).HasColumnName("TotalCalories").HasPrecision(10, 2);
            macros.Property(value => value.ProteinGrams).HasColumnName("TotalProteinGrams").HasPrecision(10, 2);
            macros.Property(value => value.CarbohydrateGrams).HasColumnName("TotalCarbohydrateGrams").HasPrecision(10, 2);
            macros.Property(value => value.FatGrams).HasColumnName("TotalFatGrams").HasPrecision(10, 2);
        });

        builder.HasMany(meal => meal.Items)
            .WithOne()
            .HasForeignKey(item => item.MealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(meal => meal.Photos)
            .WithOne()
            .HasForeignKey(photo => photo.MealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(meal => meal.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(meal => meal.Photos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(meal => new { meal.UserId, meal.OccurredAt });
    }
}
