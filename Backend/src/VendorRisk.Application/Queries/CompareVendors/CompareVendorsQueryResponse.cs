using VendorRisk.Application.DTOs;

namespace VendorRisk.Application.Queries.CompareVendors;

public record CompareVendorsQueryResponse
{
    public VendorComparisonDto Comparison { get; init; } = null!;
}
