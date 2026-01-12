using FluentValidation;

namespace VendorRisk.Application.Queries.CompareVendors;

public class CompareVendorsQueryValidator : AbstractValidator<CompareVendorsQueryRequest>
{
    public CompareVendorsQueryValidator()
    {
        RuleFor(x => x.VendorIds)
            .NotEmpty()
            .WithMessage("At least one vendor ID must be provided");

        RuleFor(x => x.VendorIds)
            .Must(ids => ids.Count >= 2)
            .WithMessage("At least two vendors must be provided for comparison");

        RuleFor(x => x.VendorIds)
            .Must(ids => ids.Count <= 10)
            .WithMessage("Maximum 10 vendors can be compared at once");

        RuleForEach(x => x.VendorIds)
            .NotEmpty()
            .WithMessage("Vendor ID cannot be empty");
    }
}
