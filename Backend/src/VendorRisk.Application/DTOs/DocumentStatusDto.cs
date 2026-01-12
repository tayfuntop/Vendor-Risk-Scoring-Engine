namespace VendorRisk.Application.DTOs;

public record DocumentStatusDto(
    bool ContractValid,
    bool PrivacyPolicyValid,
    bool PentestReportValid
);
