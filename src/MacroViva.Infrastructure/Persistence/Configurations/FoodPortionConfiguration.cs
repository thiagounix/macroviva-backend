using MacroViva.Domain.Nutrition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class FoodPortionConfiguration : IEntityTypeConfiguration<FoodPortion>
{
    public void Configure(EntityTypeBuilder<FoodPortion> builder)
    {
        builder.ToTable("FoodPortions");
        builder.HasKey(portion => portion.Id);

        builder.Property(portion => portion.FoodId)
            .IsRequired();

        builder.OwnsOne(portion => portion.Name, name =>
        {
            name.Property(value => value.Value).HasColumnName("Name").HasMaxLength(120).IsRequired();
            name.Property(value => value.Locale).HasColumnName("NameLocale").HasConversion<string>().HasMaxLength(20).IsRequired();
        });

        builder.OwnsOne(portion => portion.Portion, value =>
        {
            value.Property(portion => portion.Quantity).HasColumnName("Quantity").HasPrecision(10, 2);
            value.Property(portion => portion.Unit).HasColumnName("Unit").HasMaxLength(30).IsRequired();
            value.Property(portion => portion.Grams).HasColumnName("Grams").HasPrecision(10, 2);
        });
    }
}
