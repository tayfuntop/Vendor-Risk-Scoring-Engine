using Microsoft.EntityFrameworkCore;
using Serilog;
using VendorRisk.Api.Middleware;
using VendorRisk.Application;
using VendorRisk.Infrastructure;
using VendorRisk.Infrastructure.Persistence.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Vendor Risk API - Rule-Based Scoring Engine");

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Vendor Risk Scoring API", Version = "v1" });
    });

    // CORS Configuration for Frontend
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            //policy.WithOrigins("http://localhost:3000")
            //      .AllowAnyHeader()
            //      .AllowAnyMethod();

            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });



    // Application & Infrastructure layers (Clean Architecture)
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Middleware Pipeline
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();

    // Configure HTTP request pipeline
    // if (app.Environment.IsDevelopment())
    // {
        app.UseSwagger();
        app.UseSwaggerUI();
    // }

    // Enable CORS
    app.UseCors("AllowFrontend");

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // Database Migration and Seeding
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            Log.Information("Applying database migrations...");
            var context = services.GetRequiredService<PostgresContext>();
            await context.Database.MigrateAsync();
            Log.Information("Database migrations completed");

            Log.Information("Seeding database with sample vendor data...");
            var seeder = services.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync();
            Log.Information("Database seeding completed");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database migration or seeding failed");
            throw;
        }
    }

    Log.Information("Vendor Risk API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
