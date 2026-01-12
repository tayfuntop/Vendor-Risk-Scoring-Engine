using MediatR;
using VendorRisk.Application.DTOs;
using VendorRisk.Application.Interfaces;

namespace VendorRisk.Application.Queries.GetVendorRiskAssessment;

public class GetVendorRiskAssessmentQueryHandler : IRequestHandler<GetVendorRiskAssessmentQueryRequest, GetVendorRiskAssessmentQueryResponse>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IRiskAssessmentRepository _riskAssessmentRepository;
    private readonly IRiskCalculationService _riskCalculationService;

    public GetVendorRiskAssessmentQueryHandler(
        IVendorRepository vendorRepository,
        IRiskAssessmentRepository riskAssessmentRepository,
        IRiskCalculationService riskCalculationService)
    {
        _vendorRepository = vendorRepository;
        _riskAssessmentRepository = riskAssessmentRepository;
        _riskCalculationService = riskCalculationService;
    }

    public async Task<GetVendorRiskAssessmentQueryResponse> Handle(GetVendorRiskAssessmentQueryRequest request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {request.VendorId} not found");
        }

        var assessment = await _riskCalculationService.CalculateRiskAsync(vendor, cancellationToken);

        await _riskAssessmentRepository.AddAsync(assessment, cancellationToken);
        await _riskAssessmentRepository.SaveChangesAsync(cancellationToken);

        var assessmentDto = new RiskAssessmentDto(
            assessment.VendorId,
            assessment.RiskScore,
            assessment.RiskLevel.ToString(),
            assessment.Reason
        );

        return new GetVendorRiskAssessmentQueryResponse { Assessment = assessmentDto };
    }
}
