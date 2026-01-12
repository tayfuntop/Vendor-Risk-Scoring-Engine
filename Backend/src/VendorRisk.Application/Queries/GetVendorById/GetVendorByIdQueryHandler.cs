using MediatR;
using VendorRisk.Application.DTOs;
using VendorRisk.Application.Interfaces;

namespace VendorRisk.Application.Queries.GetVendorById;

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQueryRequest, GetVendorByIdQueryResponse>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICacheService _cacheService;

    public GetVendorByIdQueryHandler(IVendorRepository vendorRepository, ICacheService cacheService)
    {
        _vendorRepository = vendorRepository;
        _cacheService = cacheService;
    }

    public async Task<GetVendorByIdQueryResponse> Handle(GetVendorByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var cacheKey = $"vendor:{request.VendorId}";

        var cachedVendor = await _cacheService.GetAsync<VendorDto>(cacheKey, cancellationToken);
        if (cachedVendor != null)
        {
            return new GetVendorByIdQueryResponse { Vendor = cachedVendor };
        }

        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {request.VendorId} not found");
        }

        var vendorDto = new VendorDto(
            vendor.Id,
            vendor.Name,
            vendor.FinancialHealth,
            vendor.SlaUptime,
            vendor.MajorIncidents,
            vendor.SecurityCerts,
            new DocumentStatusDto(
                vendor.Documents.ContractValid,
                vendor.Documents.PrivacyPolicyValid,
                vendor.Documents.PentestReportValid)
        );

        await _cacheService.SetAsync(cacheKey, vendorDto, TimeSpan.FromMinutes(5), cancellationToken);

        return new GetVendorByIdQueryResponse { Vendor = vendorDto };
    }
}
