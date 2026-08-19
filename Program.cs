using System;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniMap.Data;
using MiniMap.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to DI
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Detect Database Availability
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
bool useSqlServer = true;

if (useSqlServer)
{
    Console.WriteLine("[Database] Connected to SQL Server on localhost:1433.");
    builder.Services.AddDbContext<TravelReviewDbContext>(options =>
    {
        options.UseSqlServer(connString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(3), errorNumbersToAdd: null);
        });
    });
}
else
{
    Console.WriteLine("[Database] Running with In-Memory Database (Fast & Resilient mode).");
    builder.Services.AddDbContext<TravelReviewDbContext>(options =>
    {
        options.UseInMemoryDatabase("TravelReviewDB");
    });
}

// Application Services
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISystemAdminService, SystemAdminService>();

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TravelReviewDbContext>();
        DbInitializer.Initialize(context);
        Console.WriteLine("[Database] Initialized and Seeded with sample Vietnamese places, categories & reviews.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB Seed Exception] {ex.Message}");
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
