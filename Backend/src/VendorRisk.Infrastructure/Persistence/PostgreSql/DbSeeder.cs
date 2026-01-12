using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VendorRisk.Domain.Common;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Infrastructure.Persistence.PostgreSql;

public class DbSeeder
{
    private readonly PostgresContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(PostgresContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.VendorProfiles.AnyAsync())
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Starting database seeding...");

        try
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "SampleVendorData.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Seed data file not found at {JsonPath}. Skipping seeding.", jsonPath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var seedData = JsonConvert.DeserializeObject<SeedDataModel>(jsonContent);

            if (seedData?.Vendors == null || !seedData.Vendors.Any())
            {
                _logger.LogWarning("No vendor data found in seed file.");
                return;
            }

            foreach (var vendorData in seedData.Vendors)
            {
                var documentStatus = new DocumentStatus(
                    vendorData.Documents.ContractValid,
                    vendorData.Documents.PrivacyPolicyValid,
                    vendorData.Documents.PentestReportValid);

                var vendor = VendorProfile.Create(
                    vendorData.Name,
                    vendorData.FinancialHealth,
                    vendorData.SlaUptime,
                    vendorData.MajorIncidents,
                    vendorData.SecurityCerts,
                    documentStatus);

                await _context.VendorProfiles.AddAsync(vendor);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Database seeding completed. {Count} vendors added.", seedData.Vendors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            throw;
        }
    }
}

// Models for JSON deserialization
public class SeedDataModel
{
    [JsonProperty("Vendors")]
    public List<VendorSeedData> Vendors { get; set; } = new();
}

public class VendorSeedData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("financialHealth")]
    public decimal FinancialHealth { get; set; }

    [JsonProperty("slaUptime")]
    public decimal SlaUptime { get; set; }

    [JsonProperty("majorIncidents")]
    public int MajorIncidents { get; set; }

    [JsonProperty("securityCerts")]
    public List<string> SecurityCerts { get; set; } = new();

    [JsonProperty("documents")]
    public DocumentsSeedData Documents { get; set; } = new();
}

public class DocumentsSeedData
{
    [JsonProperty("contractValid")]
    public bool ContractValid { get; set; }

    [JsonProperty("privacyPolicyValid")]
    public bool PrivacyPolicyValid { get; set; }

    [JsonProperty("pentestReportValid")]
    public bool PentestReportValid { get; set; }
}
