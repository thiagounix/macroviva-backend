using MacroViva.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.Locale)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasOne(user => user.Profile)
            .WithOne()
            .HasForeignKey<UserProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(user => user.Goal)
            .WithOne()
            .HasForeignKey<UserGoal>(goal => goal.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.Consents)
            .WithOne()
            .HasForeignKey(consent => consent.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(user => user.Consents)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
