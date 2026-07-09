using MacroViva.Application.AIAnalysis;
using MacroViva.Application.Foods;
using MacroViva.Application.Meals;
using MacroViva.Application.Supplements;
using MacroViva.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
const string DevelopmentCorsPolicy = "DevelopmentCors";

builder.Services.AddControllers();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
