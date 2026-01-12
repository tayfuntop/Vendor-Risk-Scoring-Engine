namespace VendorRisk.Domain.Common;

public class OperationalMetrics : ValueObject
{
    public decimal SlaUptimePercentage { get; private set; }
    public int MajorIncidentsCount { get; private set; }

    private OperationalMetrics() { }

    public OperationalMetrics(decimal slaUptimePercentage, int majorIncidentsCount)
    {
        if (slaUptimePercentage < 0 || slaUptimePercentage > 100)
            throw new ArgumentException("SLA uptime percentage must be between 0 and 100", nameof(slaUptimePercentage));

        if (majorIncidentsCount < 0)
            throw new ArgumentException("Major incidents count cannot be negative", nameof(majorIncidentsCount));

        SlaUptimePercentage = slaUptimePercentage;
        MajorIncidentsCount = majorIncidentsCount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SlaUptimePercentage;
        yield return MajorIncidentsCount;
    }
}
