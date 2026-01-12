using VendorRisk.Application.Interfaces;
using VendorRisk.Domain.Entities;
using VendorRisk.Domain.Enums;

namespace VendorRisk.Application.Services;
public class RuleEngineService : IRuleEngineService
{
    private readonly IRiskSimilarityMatrixService _similarityMatrixService;

    private const decimal FinancialWeight = 0.4m;
    private const decimal OperationalWeight = 0.3m;
    private const decimal SecurityWeight = 0.3m;

    public RuleEngineService(IRiskSimilarityMatrixService similarityMatrixService)
    {
        _similarityMatrixService = similarityMatrixService;
    }

    public RiskAssessment CalculateRisk(VendorProfile vendor)
    {
        var financialRisk = CalculateFinancialRisk(vendor);
        var operationalRisk = CalculateOperationalRisk(vendor);
        var securityRisk = CalculateSecurityRisk(vendor);

        var baseOverallRisk = (financialRisk * FinancialWeight) +
                              (operationalRisk * OperationalWeight) +
                              (securityRisk * SecurityWeight);

        var activeRiskFactors = IdentifyActiveRiskFactors(vendor);

        var correlatedRisks = _similarityMatrixService.GetCorrelatedRisks(activeRiskFactors);

        var correlationFactor = correlatedRisks.Any() ? correlatedRisks.Values.Sum() / 10m : 0m;
        var amplifiedRisk = baseOverallRisk * (1 + correlationFactor);

        amplifiedRisk = Math.Min(amplifiedRisk, 1.0m);

        var riskLevel = DetermineRiskLevel(amplifiedRisk);
        var reason = GenerateReason(vendor, riskLevel, amplifiedRisk);

        return RiskAssessment.Create(
            vendor.Id,
            amplifiedRisk,
            riskLevel,
            reason);
    }

    #region Financial Risk Rules

    private decimal CalculateFinancialRisk(VendorProfile vendor)
    {
        decimal risk;
        if (vendor.FinancialHealth < 50)
            risk = 0.75m; // High risk
        else if (vendor.FinancialHealth > 80)
            risk = 0.1m; // Low risk
        else
            risk = 0.4m; // Medium risk

        return risk;
    }

    #endregion

    #region Operational Risk Rules

    private decimal CalculateOperationalRisk(VendorProfile vendor)
    {
        var slaRisk = (100m - vendor.SlaUptime) / 100m;

        var incidentRisk = Math.Min(vendor.MajorIncidents / 10m, 1.0m);

        var risk = (slaRisk * 0.6m) + (incidentRisk * 0.4m);
        return Math.Max(0, Math.Min(1, risk));
    }

    #endregion

    #region Security & Compliance Risk Rules

    private decimal CalculateSecurityRisk(VendorProfile vendor)
    {
        if (!vendor.Documents.PentestReportValid)
            return 1.0m; // Critical override

        decimal certRisk;
        var certCount = vendor.SecurityCerts.Count;
        if (certCount == 0)
            certRisk = 1.0m; // High risk
        else if (certCount <= 2)
            certRisk = 0.5m; // Medium risk
        else
            certRisk = 0.0m; // Low risk

        var validDocs = vendor.Documents.ValidDocumentsCount;
        var docRisk = (3m - validDocs) / 3m;

        var risk = (certRisk * 0.5m) + (docRisk * 0.5m);
        return Math.Max(0, Math.Min(1, risk));
    }

    #endregion

    #region Risk Factor Identification Rules

    private List<string> IdentifyActiveRiskFactors(VendorProfile vendor)
    {
        var factors = new List<string>();

        // Financial risk factors
        if (vendor.FinancialHealth < 50)
            factors.Add("lowFinancialHealth");
        if (vendor.FinancialHealth < 60)
            factors.Add("lowCashFlow");

        // Operational risk factors
        if (vendor.MajorIncidents >= 5)
            factors.Add("highMajorIncidents");
        if (vendor.MajorIncidents >= 3)
            factors.Add("majorIncident");
        if (vendor.SlaUptime < 95)
            factors.Add("lowSlaUptime");
        if (vendor.SlaUptime < 90)
            factors.Add("slaDrop");

        // Security risk factors
        if (vendor.SecurityCerts.Count == 0)
            factors.Add("missingCertifications");
        if (!vendor.SecurityCerts.Contains("ISO27001"))
            factors.Add("missingISO27001");

        // Compliance risk factors
        if (vendor.Documents.ValidDocumentsCount < 2)
            factors.Add("invalidDocuments");
        if (!vendor.Documents.ContractValid)
            factors.Add("expiredContract");
        if (!vendor.Documents.PrivacyPolicyValid)
            factors.Add("expiredPrivacyPolicy");
        if (!vendor.Documents.PentestReportValid)
            factors.Add("failedPenTest");

        return factors;
    }

    #endregion

    #region Risk Level Determination Rules

    private RiskLevel DetermineRiskLevel(decimal overallRisk)
    {
        // Business Rules for Risk Level Classification
        return overallRisk switch
        {
            < 0.25m => RiskLevel.Low,
            < 0.50m => RiskLevel.Medium,
            < 0.75m => RiskLevel.High,
            _ => RiskLevel.Critical
        };
    }

    #endregion

    #region Explainability - Human Readable Reason

    private string GenerateReason(VendorProfile vendor, RiskLevel riskLevel, decimal riskScore)
    {
        var reasons = new List<string>();

        // Financial indicators
        if (vendor.FinancialHealth < 50)
            reasons.Add($"Low financial health ({vendor.FinancialHealth})");

        // Operational indicators
        if (vendor.SlaUptime < 95)
            reasons.Add($"SLA < 95% ({vendor.SlaUptime}%)");

        if (vendor.MajorIncidents > 0)
            reasons.Add($"{vendor.MajorIncidents} major incident(s)");

        // Security indicators
        if (!vendor.SecurityCerts.Contains("ISO27001"))
            reasons.Add("Missing ISO27001");

        if (vendor.SecurityCerts.Count == 0)
            reasons.Add("No security certifications");

        // Compliance indicators
        if (!vendor.Documents.PrivacyPolicyValid)
            reasons.Add("Privacy policy expired");

        if (!vendor.Documents.ContractValid)
            reasons.Add("Contract invalid");

        if (!vendor.Documents.PentestReportValid)
            reasons.Add("Pentest report invalid");

        // If no specific risks identified, provide a summary
        if (reasons.Count == 0)
            return $"{riskLevel} risk level - No significant risk factors identified";

        return string.Join(" + ", reasons);
    }

    #endregion
}
