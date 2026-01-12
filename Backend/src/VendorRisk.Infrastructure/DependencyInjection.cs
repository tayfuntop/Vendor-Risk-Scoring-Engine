using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VendorRisk.Application.Interfaces;
using VendorRisk.Infrastructure.Configuration;
using VendorRisk.Infrastructure.Persistence.PostgreSql;
using VendorRisk.Infrastructure.Persistence.PostgreSql.Repositories;
using VendorRisk.Infrastructure.Services;

namespace VendorRisk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL
        services.AddDbContext<PostgresContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        // Repositories
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IRiskAssessmentRepository, RiskAssessmentRepository>();

        // Redis
        var redisSettings = configuration.GetSection("Redis").Get<RedisSettings>();
        if (redisSettings != null && !string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

            services.AddSingleton<ICacheService>(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisCacheService(redis, redisSettings.InstanceName);
            });
        }

        // Risk Engine Services
        var riskEngineSettings = configuration.GetSection("RiskEngine").Get<RiskEngineSettings>();
        services.AddSingleton<IRiskSimilarityMatrixService>(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RiskSimilarityMatrixService>>();
            var matrixPath = riskEngineSettings?.MatrixFilePath ?? "Data/RiskFactorMatrix.json";
            var fullPath = Path.Combine(AppContext.BaseDirectory, matrixPath);
            return new RiskSimilarityMatrixService(fullPath, logger);
        });

        services.AddScoped<IRiskCalculationService, RiskCalculationService>();

        // Application Services (Business Logic)
        services.AddScoped<IRuleEngineService, VendorRisk.Application.Services.RuleEngineService>();

        // Database Seeder
        services.AddScoped<DbSeeder>();

        return services;
    }
}
