namespace VendorRisk.Domain.Common;

public class FinancialHealth : ValueObject
{
    public decimal Score { get; private set; }

    private FinancialHealth() { }

    public FinancialHealth(decimal score)
    {
        if (score < 0 || score > 100)
            throw new ArgumentException("Financial health score must be between 0 and 100", nameof(score));

        Score = score;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
    }
}
