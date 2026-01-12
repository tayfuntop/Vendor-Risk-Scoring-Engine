using MediatR;

namespace VendorRisk.Application.Queries.GetVendorById;

public class GetVendorByIdQueryRequest : IRequest<GetVendorByIdQueryResponse>
{
    public int VendorId { get; set; }
}
