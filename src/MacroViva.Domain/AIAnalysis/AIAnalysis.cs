using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;

namespace MacroViva.Domain.AIAnalysis;

public sealed class AIAnalysis : AggregateRoot
{
    private readonly List<AIAnalysisItem> _items = [];

    private AIAnalysis(Guid id, Guid userId, AIAnalysisStatus status, DateTimeOffset createdAt)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        Status = status;
        CreatedAt = Guard.AgainstDefault(createdAt, nameof(createdAt));
    }

    public Guid UserId { get; }

    public AIAnalysisStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? ConfirmedAt { get; private set; }

    public IReadOnlyCollection<AIAnalysisItem> Items => _items.AsReadOnly();

    public static AIAnalysis StartPending(Guid id, Guid userId, DateTimeOffset createdAt)
    {
        return new AIAnalysis(id, userId, AIAnalysisStatus.Pending, createdAt);
    }

    public static AIAnalysis CreateCompleted(
        Guid id,
        Guid userId,
        DateTimeOffset createdAt,
        DateTimeOffset completedAt,
        IEnumerable<AIAnalysisItem> items)
    {
        var analysis = new AIAnalysis(id, userId, AIAnalysisStatus.Pending, createdAt);
        analysis.Complete(items, completedAt);

        return analysis;
    }

    public void Complete(IEnumerable<AIAnalysisItem> items, DateTimeOffset completedAt)
    {
        if (Status != AIAnalysisStatus.Pending)
        {
            throw new DomainException("Only pending AI analyses can be completed.");
        }

        Guard.AgainstDefault(completedAt, nameof(completedAt));

        if (completedAt < CreatedAt)
        {
            throw new DomainException("AI analysis completion cannot happen before creation.");
        }

        var itemList = items?.ToList() ?? throw new ArgumentNullException(nameof(items));

        if (itemList.Count == 0)
        {
            throw new DomainException("AI analysis cannot be completed without suggested items.");
        }

        if (itemList.Any(item => item is null))
        {
            throw new DomainException("AI analysis cannot be completed with invalid suggested items.");
        }

        _items.AddRange(itemList);
        CompletedAt = completedAt;
        Status = AIAnalysisStatus.Completed;
    }

    public void Confirm(DateTimeOffset confirmedAt)
    {
        if (Status != AIAnalysisStatus.Completed)
        {
            throw new DomainException("Only completed AI analyses can be confirmed.");
        }

        Guard.AgainstDefault(confirmedAt, nameof(confirmedAt));

        if (!CompletedAt.HasValue)
        {
            throw new DomainException("AI analysis must be completed before confirmation.");
        }

        if (confirmedAt < CompletedAt.Value)
        {
            throw new DomainException("AI analysis confirmation cannot happen before completion.");
        }

        if (_items.Count == 0)
        {
            throw new DomainException("AI analysis cannot be confirmed without suggested items.");
        }

        ConfirmedAt = confirmedAt;
        Status = AIAnalysisStatus.Confirmed;
    }

    public void Fail(DateTimeOffset completedAt)
    {
        if (Status != AIAnalysisStatus.Pending)
        {
            throw new DomainException("Only pending AI analyses can fail.");
        }

        Guard.AgainstDefault(completedAt, nameof(completedAt));

        if (completedAt < CreatedAt)
        {
            throw new DomainException("AI analysis failure cannot happen before creation.");
        }

        CompletedAt = completedAt;
        Status = AIAnalysisStatus.Failed;
    }
}
