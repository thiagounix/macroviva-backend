using MacroViva.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");
        builder.HasKey(plan => plan.Id);

        builder.OwnsOne(plan => plan.Name, name =>
        {
            name.Property(value => value.Value).HasColumnName("Name").HasMaxLength(120).IsRequired();
            name.Property(value => value.Locale).HasColumnName("NameLocale").HasConversion<string>().HasMaxLength(20).IsRequired();
        });

        builder.Property(plan => plan.Tier)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.OwnsOne(plan => plan.Price, price =>
        {
            price.Property(value => value.Amount).HasColumnName("PriceAmount").HasPrecision(10, 2);
            price.Property(value => value.Currency).HasColumnName("PriceCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(plan => plan.Availability, availability =>
        {
            availability.Property(value => value.StartsOn).HasColumnName("AvailableStartsOn");
            availability.Property(value => value.EndsOn).HasColumnName("AvailableEndsOn");
        });
    }
}
