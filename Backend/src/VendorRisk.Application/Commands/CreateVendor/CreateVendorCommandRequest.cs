using MediatR;
using VendorRisk.Application.DTOs;

namespace VendorRisk.Application.Commands.CreateVendor;

public class CreateVendorCommandRequest : IRequest<CreateVendorCommandResponse>
{
    public string Name { get; set; } = string.Empty;
    public decimal FinancialHealth { get; set; }
    public decimal SlaUptime { get; set; }
    public int MajorIncidents { get; set; }
    public List<string> SecurityCerts { get; set; } = new();
    public DocumentStatusDto Documents { get; set; } = null!;
}
