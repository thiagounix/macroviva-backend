using MacroViva.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
{
    public void Configure(EntityTypeBuilder<UserGoal> builder)
    {
        builder.ToTable("UserGoals");
        builder.HasKey(goal => goal.Id);

        builder.Property(goal => goal.UserId)
            .IsRequired();

        builder.Property(goal => goal.Objective)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(goal => goal.ActivityLevel)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.OwnsOne(goal => goal.DailyTargets, targets =>
        {
            targets.OwnsOne(target => target.TargetMacronutrients, macros =>
            {
                macros.Property(value => value.Calories).HasColumnName("TargetCalories").HasPrecision(10, 2);
                macros.Property(value => value.ProteinGrams).HasColumnName("TargetProteinGrams").HasPrecision(10, 2);
                macros.Property(value => value.CarbohydrateGrams).HasColumnName("TargetCarbohydrateGrams").HasPrecision(10, 2);
                macros.Property(value => value.FatGrams).HasColumnName("TargetFatGrams").HasPrecision(10, 2);
            });
        });
    }
}
