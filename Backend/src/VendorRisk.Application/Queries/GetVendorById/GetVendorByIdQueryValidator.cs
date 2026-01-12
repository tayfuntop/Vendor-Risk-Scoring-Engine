using FluentValidation;

namespace VendorRisk.Application.Queries.GetVendorById;

public class GetVendorByIdQueryValidator : AbstractValidator<GetVendorByIdQueryRequest>
{
    public GetVendorByIdQueryValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty().WithMessage("Vendor ID is required");
    }
}
