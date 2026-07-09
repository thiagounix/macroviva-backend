using MacroViva.Domain.Supplements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class UserSupplementConfiguration : IEntityTypeConfiguration<UserSupplement>
{
    public void Configure(EntityTypeBuilder<UserSupplement> builder)
    {
        builder.ToTable("UserSupplements");
        builder.HasKey(userSupplement => userSupplement.Id);

        builder.Property(userSupplement => userSupplement.UserId).IsRequired();
        builder.Property(userSupplement => userSupplement.SupplementId).IsRequired();
        builder.Property(userSupplement => userSupplement.CheckInDate).IsRequired();
        builder.Property(userSupplement => userSupplement.Servings).HasPrecision(10, 2).IsRequired();

        builder.OwnsOne(userSupplement => userSupplement.MacronutrientImpact, macros =>
        {
            macros.Property(value => value.Calories).HasColumnName("ImpactCalories").HasPrecision(10, 2);
            macros.Property(value => value.ProteinGrams).HasColumnName("ImpactProteinGrams").HasPrecision(10, 2);
            macros.Property(value => value.CarbohydrateGrams).HasColumnName("ImpactCarbohydrateGrams").HasPrecision(10, 2);
            macros.Property(value => value.FatGrams).HasColumnName("ImpactFatGrams").HasPrecision(10, 2);
        });

        builder.HasIndex(userSupplement => new { userSupplement.UserId, userSupplement.SupplementId, userSupplement.CheckInDate });
    }
}
