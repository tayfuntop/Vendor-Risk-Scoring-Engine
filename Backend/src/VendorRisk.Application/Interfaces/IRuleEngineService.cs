using VendorRisk.Domain.Entities;

namespace VendorRisk.Application.Interfaces;

public interface IRuleEngineService
{
    RiskAssessment CalculateRisk(VendorProfile vendor);
}
