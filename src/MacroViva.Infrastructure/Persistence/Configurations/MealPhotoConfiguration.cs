using MacroViva.Domain.Meals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class MealPhotoConfiguration : IEntityTypeConfiguration<MealPhoto>
{
    public void Configure(EntityTypeBuilder<MealPhoto> builder)
    {
        builder.ToTable("MealPhotos");
        builder.HasKey(photo => photo.Id);

        builder.Property(photo => photo.MealId)
            .IsRequired();

        builder.Property(photo => photo.TemporaryReference)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(photo => photo.ContentType)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(photo => photo.CapturedAt)
            .IsRequired();
    }
}
