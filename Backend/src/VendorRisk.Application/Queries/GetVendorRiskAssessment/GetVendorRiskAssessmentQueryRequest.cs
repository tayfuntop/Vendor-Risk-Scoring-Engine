using MediatR;

namespace VendorRisk.Application.Queries.GetVendorRiskAssessment;

public class GetVendorRiskAssessmentQueryRequest : IRequest<GetVendorRiskAssessmentQueryResponse>
{
    public int VendorId { get; set; }
}
