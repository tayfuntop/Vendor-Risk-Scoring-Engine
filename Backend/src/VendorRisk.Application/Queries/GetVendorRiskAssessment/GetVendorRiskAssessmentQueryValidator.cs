using FluentValidation;

namespace VendorRisk.Application.Queries.GetVendorRiskAssessment;

public class GetVendorRiskAssessmentQueryValidator : AbstractValidator<GetVendorRiskAssessmentQueryRequest>
{
    public GetVendorRiskAssessmentQueryValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithMessage("Vendor ID is required");
    }
}
