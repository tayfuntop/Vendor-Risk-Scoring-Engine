namespace VendorRisk.Domain.Common;

public class RiskFactor : ValueObject
{
    public string Name { get; private set; }
    public decimal Impact { get; private set; }
    public string Description { get; private set; }

    private RiskFactor()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public RiskFactor(string name, decimal impact, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Impact = impact;
        Description = description ?? string.Empty;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Impact;
    }
}
