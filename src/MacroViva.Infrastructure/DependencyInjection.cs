using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Infrastructure.Persistence;
using MacroViva.Infrastructure.Persistence.Seed;
using MacroViva.Infrastructure.Repositories;
using MacroViva.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MacroViva.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration["ConnectionStrings:DefaultConnection"]
            ?? configuration["ConnectionStrings__DefaultConnection"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");
        }

        services.AddDbContext<MacroVivaDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IFoodRepository, FoodRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAIAnalysisRepository, AIAnalysisRepository>();
        services.AddScoped<ISupplementRepository, SupplementRepository>();
        services.AddScoped<IUserSupplementRepository, UserSupplementRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IMealVisionAnalyzer, MockMealVisionAnalyzer>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
