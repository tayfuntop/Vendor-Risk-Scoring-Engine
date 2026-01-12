namespace VendorRisk.Application.DTOs;

public record RiskAssessmentDto(
    int VendorId,
    decimal RiskScore,
    string RiskLevel,
    string Reason
);
