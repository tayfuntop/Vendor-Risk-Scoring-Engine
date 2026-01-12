using Microsoft.Extensions.Logging;
using VendorRisk.Application.Interfaces;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Infrastructure.Services;
public class RiskCalculationService : IRiskCalculationService
{
    private readonly IRuleEngineService _ruleEngineService;
    private readonly ILogger<RiskCalculationService> _logger;

    public RiskCalculationService(
        IRuleEngineService ruleEngineService,
        ILogger<RiskCalculationService> logger)
    {
        _ruleEngineService = ruleEngineService;
        _logger = logger;
    }

    public Task<RiskAssessment> CalculateRiskAsync(VendorProfile vendor, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating risk for vendor {VendorId} - {VendorName}", vendor.Id, vendor.Name);

        var assessment = _ruleEngineService.CalculateRisk(vendor);

        _logger.LogInformation(
            "Risk calculated for vendor {VendorId}: Overall={Overall:P2}, Level={Level}",
            vendor.Id,
            assessment.RiskScore,
            assessment.RiskLevel);

        return Task.FromResult(assessment);
    }
}
