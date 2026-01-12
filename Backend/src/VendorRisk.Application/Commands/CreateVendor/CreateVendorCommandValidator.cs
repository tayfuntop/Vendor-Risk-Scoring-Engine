using FluentValidation;

namespace VendorRisk.Application.Commands.CreateVendor;

public class CreateVendorCommandValidator : AbstractValidator<CreateVendorCommandRequest>
{
    public CreateVendorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Vendor name is required")
            .MaximumLength(255).WithMessage("Vendor name cannot exceed 255 characters");

        RuleFor(x => x.FinancialHealth)
            .InclusiveBetween(0, 100).WithMessage("Financial health must be between 0 and 100");

        RuleFor(x => x.SlaUptime)
            .InclusiveBetween(0, 100).WithMessage("SLA uptime must be between 0 and 100");

        RuleFor(x => x.MajorIncidents)
            .GreaterThanOrEqualTo(0).WithMessage("Major incidents cannot be negative");

        RuleFor(x => x.SecurityCerts)
            .NotNull().WithMessage("Security certifications list cannot be null");

        RuleFor(x => x.Documents)
            .NotNull().WithMessage("Document status is required");
    }
}
