using VendorRisk.Application.DTOs;

namespace VendorRisk.Application.Queries.GetVendorRiskAssessment;

public class GetVendorRiskAssessmentQueryResponse
{
    public RiskAssessmentDto? Assessment { get; set; }
}
