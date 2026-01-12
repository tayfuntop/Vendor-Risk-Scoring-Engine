using VendorRisk.Domain.Entities;

namespace VendorRisk.Application.Interfaces;

public interface IRiskCalculationService
{
    Task<RiskAssessment> CalculateRiskAsync(VendorProfile vendor, CancellationToken cancellationToken = default);
}
