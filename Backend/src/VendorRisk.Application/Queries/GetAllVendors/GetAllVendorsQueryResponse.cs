using VendorRisk.Application.DTOs;

namespace VendorRisk.Application.Queries.GetAllVendors;

public class GetAllVendorsQueryResponse
{
    public List<VendorDto> Vendors { get; set; } = new();
}
