namespace VendorRisk.Application.DTOs;

public record VendorDto(
    int Id,
    string Name,
    decimal FinancialHealth,
    decimal SlaUptime,
    int MajorIncidents,
    List<string> SecurityCerts,
    DocumentStatusDto Documents
);
