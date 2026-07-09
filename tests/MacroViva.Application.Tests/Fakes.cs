using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Domain.Meals;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.Supplements;
using MacroViva.Domain.Users;
using MealImageAnalysis = MacroViva.Domain.AIAnalysis.AIAnalysis;

namespace MacroViva.Application.Tests;

internal sealed class FakeFoodRepository : IFoodRepository
{
    private readonly Dictionary<Guid, Food> _foods = [];

    public void Add(Food food)
    {
        _foods[food.Id] = food;
    }

    public Task<IReadOnlyList<Food>> SearchAsync(string? search, CancellationToken cancellationToken)
    {
        var foods = _foods.Values
            .Where(food => string.IsNullOrWhiteSpace(search)
                || food.Name.Value.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult<IReadOnlyList<Food>>(foods);
    }

    public Task<Food?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _foods.TryGetValue(id, out var food);

        return Task.FromResult(food);
    }
}

internal sealed class FakeMealRepository : IMealRepository
{
    public List<Meal> AddedMeals { get; } = [];

    public Task AddAsync(Meal meal, CancellationToken cancellationToken)
    {
        AddedMeals.Add(meal);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Meal>> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken)
    {
        var meals = AddedMeals
            .Where(meal => meal.UserId == userId && DateOnly.FromDateTime(meal.OccurredAt.UtcDateTime) == date)
            .ToList();

        return Task.FromResult<IReadOnlyList<Meal>>(meals);
    }
}

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = [];

    public void Add(User user)
    {
        _users[user.Id] = user;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _users.TryGetValue(id, out var user);

        return Task.FromResult(user);
    }
}

internal sealed class FakeAIAnalysisRepository : IAIAnalysisRepository
{
    private readonly Dictionary<Guid, MealImageAnalysis> _analyses = [];

    public IReadOnlyCollection<MealImageAnalysis> Analyses => _analyses.Values;

    public int UpdateCalls { get; private set; }

    public Task AddAsync(MealImageAnalysis analysis, CancellationToken cancellationToken)
    {
        _analyses[analysis.Id] = analysis;

        return Task.CompletedTask;
    }

    public void AddExisting(MealImageAnalysis analysis)
    {
        _analyses[analysis.Id] = analysis;
    }

    public Task<MealImageAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _analyses.TryGetValue(id, out var analysis);

        return Task.FromResult(analysis);
    }

    public void Update(MealImageAnalysis analysis)
    {
        _analyses[analysis.Id] = analysis;
        UpdateCalls++;
    }
}

internal sealed class FakeSupplementRepository : ISupplementRepository
{
    private readonly Dictionary<Guid, Supplement> _supplements = [];

    public void Add(Supplement supplement)
    {
        _supplements[supplement.Id] = supplement;
    }

    public Task<IReadOnlyList<Supplement>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Supplement>>(_supplements.Values.ToList());
    }

    public Task<Supplement?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _supplements.TryGetValue(id, out var supplement);

        return Task.FromResult(supplement);
    }
}

internal sealed class FakeUserSupplementRepository : IUserSupplementRepository
{
    public List<UserSupplement> Added { get; } = [];

    public int UpdateCalls { get; private set; }

    public Task AddAsync(UserSupplement userSupplement, CancellationToken cancellationToken)
    {
        Added.Add(userSupplement);

        return Task.CompletedTask;
    }

    public Task<UserSupplement?> GetByUserAndSupplementAsync(Guid userId, Guid supplementId, CancellationToken cancellationToken)
    {
        var checkIn = Added.FirstOrDefault(item => item.UserId == userId && item.SupplementId == supplementId);

        return Task.FromResult(checkIn);
    }

    public void Update(UserSupplement userSupplement)
    {
        UpdateCalls++;
    }
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCalls++;

        return Task.FromResult(1);
    }
}

internal sealed class FakeMealVisionAnalyzer(IReadOnlyList<MealVisionDetectedItem> items) : IMealVisionAnalyzer
{
    public Task<MealVisionAnalysisResult> AnalyzeAsync(MealVisionAnalysisRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new MealVisionAnalysisResult(items));
    }
}

internal sealed class FakeFileStorageService : IFileStorageService
{
    public Task<FileStorageResult> SaveAsync(FileStorageRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new FileStorageResult("storage://meal-photo", request.FileName, request.ContentType));
    }
}

internal sealed class FakeCurrentUserService(Guid userId, bool isAuthenticated = true) : ICurrentUserService
{
    public Guid UserId { get; } = userId;

    public bool IsAuthenticated { get; } = isAuthenticated;
}

internal sealed class FakeClock(DateTimeOffset utcNow, DateOnly today) : IClock
{
    public DateTimeOffset UtcNow { get; } = utcNow;

    public DateOnly Today { get; } = today;
}
