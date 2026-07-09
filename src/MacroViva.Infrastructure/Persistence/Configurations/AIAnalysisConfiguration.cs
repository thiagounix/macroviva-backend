using MacroViva.Domain.AIAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroViva.Infrastructure.Persistence.Configurations;

public sealed class AIAnalysisConfiguration : IEntityTypeConfiguration<AIAnalysis>
{
    public void Configure(EntityTypeBuilder<AIAnalysis> builder)
    {
        builder.ToTable("AIAnalyses");
        builder.HasKey(analysis => analysis.Id);

        builder.Property(analysis => analysis.UserId)
            .IsRequired();

        builder.Property(analysis => analysis.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(analysis => analysis.CreatedAt)
            .IsRequired();

        builder.Property(analysis => analysis.CompletedAt);
        builder.Property(analysis => analysis.ConfirmedAt);

        builder.Property<string>("Provider")
            .HasMaxLength(80)
            .HasDefaultValue("mock");

        builder.Property<string>("Model")
            .HasMaxLength(120)
            .HasDefaultValue("mock-meal-vision-v1");

        builder.Property<decimal?>("OverallConfidence")
            .HasPrecision(5, 4);

        builder.Property<string>("RawResponseReference")
            .HasMaxLength(512);

        builder.HasMany(analysis => analysis.Items)
            .WithOne()
            .HasForeignKey("AIAnalysisId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(analysis => analysis.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(analysis => new { analysis.UserId, analysis.CreatedAt });
    }
}
