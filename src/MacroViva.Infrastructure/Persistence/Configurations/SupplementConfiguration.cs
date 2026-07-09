using MacroViva.Domain.Supplements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class SupplementConfiguration : IEntityTypeConfiguration<Supplement>
{
    public void Configure(EntityTypeBuilder<Supplement> builder)
    {
        builder.ToTable("Supplements");
        builder.HasKey(supplement => supplement.Id);

        builder.OwnsOne(supplement => supplement.Name, name =>
        {
            name.Property(value => value.Value).HasColumnName("Name").HasMaxLength(180).IsRequired();
            name.Property(value => value.Locale).HasColumnName("NameLocale").HasConversion<string>().HasMaxLength(20).IsRequired();
        });

        builder.Property(supplement => supplement.Type)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.OwnsOne(supplement => supplement.MacronutrientsPerServing, macros =>
        {
            macros.Property(value => value.Calories).HasColumnName("CaloriesPerServing").HasPrecision(10, 2);
            macros.Property(value => value.ProteinGrams).HasColumnName("ProteinGramsPerServing").HasPrecision(10, 2);
            macros.Property(value => value.CarbohydrateGrams).HasColumnName("CarbohydrateGramsPerServing").HasPrecision(10, 2);
            macros.Property(value => value.FatGrams).HasColumnName("FatGramsPerServing").HasPrecision(10, 2);
        });

        builder.Ignore(supplement => supplement.ImpactsMacronutrients);
    }
}
