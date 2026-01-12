using Microsoft.EntityFrameworkCore;
using VendorRisk.Domain.Entities;
using VendorRisk.Infrastructure.Persistence.PostgreSql.Configurations;

namespace VendorRisk.Infrastructure.Persistence.PostgreSql;

public class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
    }

    public DbSet<VendorProfile> VendorProfiles => Set<VendorProfile>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL table naming convention (lowercase with underscores)
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Table names to lowercase
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

            // Column names to lowercase
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLowerInvariant());
            }

            // Foreign key names to lowercase
            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.ToLowerInvariant());
            }

            // Index names to lowercase
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName()?.ToLowerInvariant());
            }
        }

        modelBuilder.ApplyConfiguration(new VendorProfileConfiguration());
        modelBuilder.ApplyConfiguration(new RiskAssessmentConfiguration());
    }
}
