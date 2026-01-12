namespace VendorRisk.Application.DTOs;

public record RiskBreakdownDto(
    List<RiskFactorDto> Factors,
    Dictionary<string, decimal> CorrelatedRisks
);
