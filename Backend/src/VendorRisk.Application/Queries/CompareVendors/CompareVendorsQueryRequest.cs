using MediatR;

namespace VendorRisk.Application.Queries.CompareVendors;

public record CompareVendorsQueryRequest : IRequest<CompareVendorsQueryResponse>
{
    public List<int> VendorIds { get; init; } = new();
}
