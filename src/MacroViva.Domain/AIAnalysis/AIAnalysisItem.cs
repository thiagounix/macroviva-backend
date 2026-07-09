using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.AIAnalysis;

public sealed class AIAnalysisItem : Entity
{
    private AIAnalysisItem()
    {
        SuggestedFoodName = string.Empty;
        EstimatedPortion = null!;
    }

    private AIAnalysisItem(
        Guid id,
        string suggestedFoodName,
        Portion estimatedPortion,
        ConfidenceLevel confidenceLevel,
        decimal confidenceScore,
        Guid? suggestedFoodId)
        : base(id)
    {
        SuggestedFoodName = Guard.AgainstNullOrWhiteSpace(suggestedFoodName, nameof(suggestedFoodName));
        EstimatedPortion = estimatedPortion ?? throw new ArgumentNullException(nameof(estimatedPortion));
        ConfidenceLevel = confidenceLevel;
        ConfidenceScore = Guard.AgainstNegative(confidenceScore, nameof(confidenceScore));

        if (ConfidenceScore > 1)
        {
            throw new DomainException("confidenceScore must be between 0 and 1.");
        }

        SuggestedFoodId = suggestedFoodId;
    }

    public string SuggestedFoodName { get; }

    public Portion EstimatedPortion { get; }

    public ConfidenceLevel ConfidenceLevel { get; }

    public decimal ConfidenceScore { get; }

    public Guid? SuggestedFoodId { get; }

    public static AIAnalysisItem Create(
        Guid id,
        string suggestedFoodName,
        Portion estimatedPortion,
        ConfidenceLevel confidenceLevel,
        decimal confidenceScore,
        Guid? suggestedFoodId = null)
    {
        return new AIAnalysisItem(id, suggestedFoodName, estimatedPortion, confidenceLevel, confidenceScore, suggestedFoodId);
    }
}
