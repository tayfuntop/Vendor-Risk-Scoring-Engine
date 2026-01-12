using MediatR;
using VendorRisk.Application.Interfaces;
using VendorRisk.Domain.Common;
using VendorRisk.Domain.Entities;

namespace VendorRisk.Application.Commands.CreateVendor;

public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommandRequest, CreateVendorCommandResponse>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICacheService _cacheService;

    public CreateVendorCommandHandler(IVendorRepository vendorRepository, ICacheService cacheService)
    {
        _vendorRepository = vendorRepository;
        _cacheService = cacheService;
    }

    public async Task<CreateVendorCommandResponse> Handle(CreateVendorCommandRequest request, CancellationToken cancellationToken)
    {
        var documentStatus = new DocumentStatus(
            request.Documents.ContractValid,
            request.Documents.PrivacyPolicyValid,
            request.Documents.PentestReportValid);

        var vendor = VendorProfile.Create(
            request.Name,
            request.FinancialHealth,
            request.SlaUptime,
            request.MajorIncidents,
            request.SecurityCerts,
            documentStatus);

        await _vendorRepository.AddAsync(vendor, cancellationToken);
        await _vendorRepository.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync("vendors:all", cancellationToken);

        return new CreateVendorCommandResponse
        {
            Id = vendor.Id,
            Name = vendor.Name
        };
    }
}
