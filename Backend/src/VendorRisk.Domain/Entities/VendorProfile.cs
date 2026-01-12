using VendorRisk.Domain.Common;

namespace VendorRisk.Domain.Entities;

public class VendorProfile
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal FinancialHealth { get; private set; }
    public decimal SlaUptime { get; private set; }
    public int MajorIncidents { get; private set; }
    public List<string> SecurityCerts { get; private set; }
    public DocumentStatus Documents { get; private set; }

    // Navigation
    private readonly List<RiskAssessment> _riskAssessments = new();
    public IReadOnlyCollection<RiskAssessment> RiskAssessments => _riskAssessments.AsReadOnly();

    private VendorProfile()
    {
        Name = string.Empty;
        SecurityCerts = new List<string>();
        Documents = null!;
    }

    private VendorProfile(
        string name,
        decimal financialHealth,
        decimal slaUptime,
        int majorIncidents,
        List<string> securityCerts,
        DocumentStatus documents)
    {
        Name = name;
        FinancialHealth = financialHealth;
        SlaUptime = slaUptime;
        MajorIncidents = majorIncidents;
        SecurityCerts = securityCerts;
        Documents = documents;
    }

    public static VendorProfile Create(
        string name,
        decimal financialHealth,
        decimal slaUptime,
        int majorIncidents,
        List<string> securityCerts,
        DocumentStatus documents)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vendor name is required", nameof(name));

        if (name.Length > 255)
            throw new ArgumentException("Vendor name cannot exceed 255 characters", nameof(name));

        if (financialHealth < 0 || financialHealth > 100)
            throw new ArgumentException("Financial health must be between 0 and 100", nameof(financialHealth));

        if (slaUptime < 0 || slaUptime > 100)
            throw new ArgumentException("SLA uptime must be between 0 and 100", nameof(slaUptime));

        if (majorIncidents < 0)
            throw new ArgumentException("Major incidents cannot be negative", nameof(majorIncidents));

        if (documents == null)
            throw new ArgumentNullException(nameof(documents));

        return new VendorProfile(name, financialHealth, slaUptime, majorIncidents, securityCerts ?? new List<string>(), documents);
    }
}
