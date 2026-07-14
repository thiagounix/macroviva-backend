using System.Text.Json.Serialization;
using MacroViva.Api.Middleware;
using MacroViva.Api.Services;
using MacroViva.Application.AIAnalysis;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Foods;
using MacroViva.Application.Meals;
using MacroViva.Application.Supplements;
using MacroViva.Infrastructure;
using MacroViva.Infrastructure.Persistence.Seed;
using MacroViva.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
const string DevelopmentCorsPolicy = "DevelopmentCors";
const string StagingCorsPolicy = "StagingCors";

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: true));
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        DevelopmentCorsPolicy,
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

    options.AddPolicy(
        StagingCorsPolicy,
        policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            if (allowedOrigins.Length == 0)
            {
                policy.SetIsOriginAllowed(_ => false);
                return;
            }

            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();
}
else if (builder.Environment.IsStaging())
{
    var hashingKey = builder.Configuration["BetaTesterIdentity:HashingKey"];

    if (string.IsNullOrWhiteSpace(hashingKey) || hashingKey.Length < 32)
    {
        throw new InvalidOperationException(
            "BetaTesterIdentity:HashingKey must be configured with at least 32 characters in Staging.");
    }

    builder.Services.AddScoped<IAnonymousBetaUserResolver, AnonymousBetaUserResolver>();
    builder.Services.AddScoped<ICurrentUserService, StagingHeaderCurrentUserService>();
}
else
{
    builder.Services.AddScoped<ICurrentUserService, UnconfiguredCurrentUserService>();
}

builder.Services.AddScoped<SearchFoodsUseCase>();
builder.Services.AddScoped<GetFoodByIdUseCase>();
builder.Services.AddScoped<CreateManualMealUseCase>();
builder.Services.AddScoped<GetTodayMealsUseCase>();
builder.Services.AddScoped<AnalyzeMealPhotoUseCase>();
builder.Services.AddScoped<ConfirmMealAnalysisUseCase>();
builder.Services.AddScoped<GetSupplementsUseCase>();
builder.Services.AddScoped<CheckInUserSupplementUseCase>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    if (app.Configuration.GetValue<bool>("OpenApi:Enabled", app.Environment.IsDevelopment()))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (app.Configuration.GetValue<bool>("Seed:RunOnStartup"))
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedDevelopmentDataAsync(app.Lifetime.ApplicationStopping);
    }
}

app.UseHttpsRedirection();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}
else if (app.Environment.IsStaging())
{
    app.UseCors(StagingCorsPolicy);
    app.UseMiddleware<StagingTesterIdentityMiddleware>();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
