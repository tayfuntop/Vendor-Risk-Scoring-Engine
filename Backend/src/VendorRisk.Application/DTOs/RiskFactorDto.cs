namespace VendorRisk.Application.DTOs;

public record RiskFactorDto(
    string Name,
    decimal Impact,
    string Description
);
