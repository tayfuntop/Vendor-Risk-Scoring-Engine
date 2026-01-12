using FluentValidation;

namespace VendorRisk.Application.Queries.GetAllVendors;

public class GetAllVendorsQueryValidator : AbstractValidator<GetAllVendorsQueryRequest>
{
    public GetAllVendorsQueryValidator()
    {
    }
}
