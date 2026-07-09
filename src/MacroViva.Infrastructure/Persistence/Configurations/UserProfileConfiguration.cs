using MacroViva.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");
        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.UserId)
            .IsRequired();

        builder.OwnsOne(profile => profile.DisplayName, displayName =>
        {
            displayName.Property(name => name.Value)
                .HasColumnName("DisplayName")
                .HasMaxLength(160)
                .IsRequired();

            displayName.Property(name => name.Locale)
                .HasColumnName("DisplayNameLocale")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(profile => profile.BodyMetrics, metrics =>
        {
            metrics.Property(value => value.WeightKg)
                .HasColumnName("WeightKg")
                .HasPrecision(8, 2);

            metrics.Property(value => value.HeightCm)
                .HasColumnName("HeightCm")
                .HasPrecision(8, 2);

            metrics.Property(value => value.BodyFatPercentage)
                .HasColumnName("BodyFatPercentage")
                .HasPrecision(5, 2);
        });
    }
}
