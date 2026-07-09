using MacroViva.Domain.AIAnalysis;
using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;
using MealImageAnalysis = MacroViva.Domain.AIAnalysis.AIAnalysis;

namespace MacroViva.Domain.Tests;

public sealed class AIAnalysisTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset CompletedAt = CreatedAt.AddMinutes(1);
    private static readonly DateTimeOffset ConfirmedAt = CompletedAt.AddMinutes(1);

    [Fact]
    public void ShouldNotConfirmPendingAnalysis()
    {
        var analysis = MealImageAnalysis.StartPending(Guid.NewGuid(), Guid.NewGuid(), CreatedAt);

        Assert.Throws<DomainException>(() => analysis.Confirm(ConfirmedAt));
    }

    [Fact]
    public void ShouldNotCompleteAnalysisWithoutSuggestedItems()
    {
        var analysis = MealImageAnalysis.StartPending(Guid.NewGuid(), Guid.NewGuid(), CreatedAt);

        Assert.Throws<DomainException>(() => analysis.Complete([], CompletedAt));
    }

    [Fact]
    public void ShouldNotCompleteAnalysisWithInvalidSuggestedItems()
    {
        var analysis = MealImageAnalysis.StartPending(Guid.NewGuid(), Guid.NewGuid(), CreatedAt);

        Assert.Throws<DomainException>(() => analysis.Complete([null!], CompletedAt));
    }

    [Fact]
    public void ShouldNotCompleteAnalysisBeforeCreation()
    {
        var analysis = MealImageAnalysis.StartPending(Guid.NewGuid(), Guid.NewGuid(), CreatedAt);
        var item = CreateItem();

        Assert.Throws<DomainException>(() => analysis.Complete([item], CreatedAt.AddTicks(-1)));
    }

    [Fact]
    public void ShouldNotConfirmAnalysisBeforeCompletion()
    {
        var item = CreateItem();
        var analysis = MealImageAnalysis.CreateCompleted(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreatedAt,
            CompletedAt,
            [item]);

        Assert.Throws<DomainException>(() => analysis.Confirm(CompletedAt.AddTicks(-1)));
    }

    [Fact]
    public void ShouldNotConfirmAnalysisTwice()
    {
        var item = CreateItem();
        var analysis = MealImageAnalysis.CreateCompleted(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreatedAt,
            CompletedAt,
            [item]);

        analysis.Confirm(ConfirmedAt);

        Assert.Throws<DomainException>(() => analysis.Confirm(ConfirmedAt.AddMinutes(1)));
    }

    [Fact]
    public void ShouldConfirmCompletedAnalysisWithSuggestedItems()
    {
        var item = CreateItem();

        var analysis = MealImageAnalysis.CreateCompleted(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CreatedAt,
            CompletedAt,
            [item]);

        analysis.Confirm(ConfirmedAt);

        Assert.Equal(AIAnalysisStatus.Confirmed, analysis.Status);
    }

    [Fact]
    public void SuggestedFoodIdShouldBeOnlyASuggestion()
    {
        var suggestedFoodId = Guid.NewGuid();

        var item = AIAnalysisItem.Create(
            Guid.NewGuid(),
            "Rice",
            Portion.FromGrams(120m),
            ConfidenceLevel.High,
            confidenceScore: 0.91m,
            suggestedFoodId);

        Assert.Equal(suggestedFoodId, item.SuggestedFoodId);
    }

    private static AIAnalysisItem CreateItem()
    {
        return AIAnalysisItem.Create(
            Guid.NewGuid(),
            "Rice",
            Portion.FromGrams(120m),
            ConfidenceLevel.High,
            confidenceScore: 0.91m);
    }
}
