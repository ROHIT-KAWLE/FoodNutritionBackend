using FoodNutritionBackend.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// Database
// ==================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// ==================================================
// API
// ==================================================

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// ==================================================
// In-memory cache
// ==================================================

builder.Services.AddMemoryCache();

// ==================================================
// CORS
// ==================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "DevelopmentCors",
        policy =>
        {
            policy
                .SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrWhiteSpace(origin))
                    {
                        return false;
                    }

                    return origin.StartsWith(
                        "http://localhost:",
                        StringComparison.OrdinalIgnoreCase)
                        ||
                        origin.StartsWith(
                        "https://localhost:",
                        StringComparison.OrdinalIgnoreCase);
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// ==================================================
// Global error handling
// ==================================================

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler =
            context.Features.Get<IExceptionHandlerFeature>();

        var exception =
            exceptionHandler?.Error;

        context.Response.StatusCode = 500;
        context.Response.ContentType =
            "application/json";

        await context.Response.WriteAsJsonAsync(
            new
            {
                error = "Internal server error.",
                detail =
                    app.Environment.IsDevelopment()
                        ? exception?.Message
                        : null
            });
    });
});

// ==================================================
// OpenAPI
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ==================================================
// CORS
// ==================================================

app.UseCors("DevelopmentCors");

// ==================================================
// Controllers
// ==================================================

app.MapControllers();

// ==================================================
// Root health
// ==================================================

app.MapGet("/", () =>
{
    return Results.Ok(
        new
        {
            status = "ok",
            service = "Food Nutrition Backend",
            version = "1.0"
        });
});

// ==================================================
// Database health
// ==================================================

app.MapGet(
    "/health",
    async (
        AppDbContext db,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var canConnect =
                await db.Database.CanConnectAsync(
                    cancellationToken);

            if (!canConnect)
            {
                return Results.Json(
                    new
                    {
                        status = "error",
                        database = "unavailable"
                    },
                    statusCode: 503);
            }

            return Results.Ok(
                new
                {
                    status = "ok",
                    database = "connected"
                });
        }
        catch (Exception ex)
        {
            return Results.Json(
                new
                {
                    status = "error",
                    database = "unavailable",
                    detail =
                        app.Environment.IsDevelopment()
                            ? ex.Message
                            : null
                },
                statusCode: 503);
        }
    });

app.Run();