namespace VendorRisk.Domain.Common;

public class SecurityCompliance : ValueObject
{
    public List<string> Certifications { get; private set; }
    public DocumentStatus Documents { get; private set; }

    private SecurityCompliance()
    {
        Certifications = new List<string>();
        Documents = null!;
    }

    public SecurityCompliance(List<string> certifications, DocumentStatus documents)
    {
        Certifications = certifications ?? new List<string>();
        Documents = documents ?? throw new ArgumentNullException(nameof(documents));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var cert in Certifications.OrderBy(c => c))
            yield return cert;

        yield return Documents;
    }
}
