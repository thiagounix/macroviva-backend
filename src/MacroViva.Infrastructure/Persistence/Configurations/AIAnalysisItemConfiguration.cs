using MacroViva.Domain.AIAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class AIAnalysisItemConfiguration : IEntityTypeConfiguration<AIAnalysisItem>
{
    public void Configure(EntityTypeBuilder<AIAnalysisItem> builder)
    {
        builder.ToTable("AIAnalysisItems");
        builder.HasKey(item => item.Id);

        builder.Property<Guid>("AIAnalysisId")
            .IsRequired();

        builder.Property(item => item.SuggestedFoodName)
            .HasMaxLength(180)
            .IsRequired();

        builder.OwnsOne(item => item.EstimatedPortion, portion =>
        {
            portion.Property(value => value.Quantity).HasColumnName("EstimatedQuantity").HasPrecision(10, 2);
            portion.Property(value => value.Unit).HasColumnName("EstimatedUnit").HasMaxLength(30).IsRequired();
            portion.Property(value => value.Grams).HasColumnName("EstimatedGrams").HasPrecision(10, 2);
        });

        builder.Property(item => item.ConfidenceLevel)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(item => item.ConfidenceScore)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(item => item.SuggestedFoodId);
    }
}
