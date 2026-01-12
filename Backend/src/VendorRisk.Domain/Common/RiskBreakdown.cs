namespace VendorRisk.Domain.Common;

public class RiskBreakdown : ValueObject
{
    public List<RiskFactor> Factors { get; private set; }
    public Dictionary<string, decimal> CorrelatedRisks { get; private set; }

    private RiskBreakdown()
    {
        Factors = new List<RiskFactor>();
        CorrelatedRisks = new Dictionary<string, decimal>();
    }

    public RiskBreakdown(List<RiskFactor> factors, Dictionary<string, decimal> correlatedRisks)
    {
        Factors = factors ?? new List<RiskFactor>();
        CorrelatedRisks = correlatedRisks ?? new Dictionary<string, decimal>();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var factor in Factors.OrderBy(f => f.Name))
            yield return factor;
    }
}
