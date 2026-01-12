using MediatR;
using VendorRisk.Application.DTOs;
using VendorRisk.Application.Interfaces;

namespace VendorRisk.Application.Queries.GetAllVendors;

public class GetAllVendorsQueryHandler : IRequestHandler<GetAllVendorsQueryRequest, GetAllVendorsQueryResponse>
{
    private readonly IVendorRepository _vendorRepository;

    public GetAllVendorsQueryHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<GetAllVendorsQueryResponse> Handle(GetAllVendorsQueryRequest request, CancellationToken cancellationToken)
    {
        var vendors = await _vendorRepository.GetAllAsync(cancellationToken);

        var vendorDtos = vendors.Select(v => new VendorDto(
            v.Id,
            v.Name,
            v.FinancialHealth,
            v.SlaUptime,
            v.MajorIncidents,
            v.SecurityCerts,
            new DocumentStatusDto(
                v.Documents.ContractValid,
                v.Documents.PrivacyPolicyValid,
                v.Documents.PentestReportValid)
        )).ToList();

        return new GetAllVendorsQueryResponse
        {
            Vendors = vendorDtos
        };
    }
}
