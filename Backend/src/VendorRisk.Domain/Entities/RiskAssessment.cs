using VendorRisk.Domain.Common;
using VendorRisk.Domain.Enums;

namespace VendorRisk.Domain.Entities;

public class RiskAssessment
{
    public int Id { get; private set; }
    public int VendorId { get; private set; }
    public VendorProfile Vendor { get; private set; }
    public decimal RiskScore { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public string Reason { get; private set; }
    public DateTime AssessedAt { get; private set; }

    private RiskAssessment()
    {
        Vendor = null!;
        Reason = string.Empty;
    }

    private RiskAssessment(
        int vendorId,
        decimal riskScore,
        RiskLevel riskLevel,
        string reason)
    {
        VendorId = vendorId;
        RiskScore = riskScore;
        RiskLevel = riskLevel;
        Reason = reason;
        AssessedAt = DateTime.UtcNow;
    }

    public static RiskAssessment Create(
        int vendorId,
        decimal riskScore,
        RiskLevel riskLevel,
        string reason)
    {
        if (vendorId <= 0)
            throw new ArgumentException("Vendor ID must be positive", nameof(vendorId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required", nameof(reason));

        return new RiskAssessment(vendorId, riskScore, riskLevel, reason);
    }
}
