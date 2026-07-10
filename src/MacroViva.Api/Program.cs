using System.Text.Json.Serialization;
using MacroViva.Application.AIAnalysis;
using MacroViva.Application.Foods;
using MacroViva.Application.Meals;
using MacroViva.Application.Supplements;
using MacroViva.Infrastructure;
using MacroViva.Infrastructure.Persistence.Seed;
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

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}
else if (app.Environment.IsStaging())
{
    app.UseCors(StagingCorsPolicy);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
