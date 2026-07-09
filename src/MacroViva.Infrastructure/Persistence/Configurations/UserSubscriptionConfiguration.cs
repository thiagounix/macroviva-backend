using MacroViva.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");
        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.UserId).IsRequired();
        builder.Property(subscription => subscription.SubscriptionPlanId).IsRequired();

        builder.Property(subscription => subscription.TierSnapshot)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.OwnsOne(subscription => subscription.PriceSnapshot, price =>
        {
            price.Property(value => value.Amount).HasColumnName("PriceSnapshotAmount").HasPrecision(10, 2);
            price.Property(value => value.Currency).HasColumnName("PriceSnapshotCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(subscription => subscription.ActivePeriod, period =>
        {
            period.Property(value => value.StartsOn).HasColumnName("ActiveStartsOn");
            period.Property(value => value.EndsOn).HasColumnName("ActiveEndsOn");
        });

        builder.HasIndex(subscription => new { subscription.UserId, subscription.SubscriptionPlanId });
    }
}
