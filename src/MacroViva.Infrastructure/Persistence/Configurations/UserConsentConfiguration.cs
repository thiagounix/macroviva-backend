using MacroViva.Domain.ConsentAndPrivacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
{
    public void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        builder.ToTable("UserConsents");
        builder.HasKey(consent => consent.Id);

        builder.Property(consent => consent.UserId)
            .IsRequired();

        builder.Property(consent => consent.ConsentKey)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(consent => consent.IsGranted)
            .IsRequired();

        builder.Property(consent => consent.DecidedAt)
            .IsRequired();

        builder.HasIndex(consent => new { consent.UserId, consent.ConsentKey, consent.DecidedAt });
    }
}
