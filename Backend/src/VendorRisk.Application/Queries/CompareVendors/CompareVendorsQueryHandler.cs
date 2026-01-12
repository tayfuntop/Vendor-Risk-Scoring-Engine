using MediatR;
using VendorRisk.Application.DTOs;
using VendorRisk.Application.Interfaces;

namespace VendorRisk.Application.Queries.CompareVendors;

public class CompareVendorsQueryHandler : IRequestHandler<CompareVendorsQueryRequest, CompareVendorsQueryResponse>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IRiskCalculationService _riskCalculationService;
    private readonly IRiskAssessmentRepository _riskAssessmentRepository;

    public CompareVendorsQueryHandler(
        IVendorRepository vendorRepository,
        IRiskCalculationService riskCalculationService,
        IRiskAssessmentRepository riskAssessmentRepository)
    {
        _vendorRepository = vendorRepository;
        _riskCalculationService = riskCalculationService;
        _riskAssessmentRepository = riskAssessmentRepository;
    }

    public async Task<CompareVendorsQueryResponse> Handle(CompareVendorsQueryRequest request, CancellationToken cancellationToken)
    {
        var vendorComparisons = new List<VendorRiskComparisonItemDto>();

        foreach (var vendorId in request.VendorIds)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId, cancellationToken);
            if (vendor == null) continue;

            var assessment = await _riskCalculationService.CalculateRiskAsync(vendor, cancellationToken);
            await _riskAssessmentRepository.AddAsync(assessment, cancellationToken);

            var strengths = IdentifyStrengths(vendor, assessment);
            var weaknesses = IdentifyWeaknesses(vendor, assessment);

            vendorComparisons.Add(new VendorRiskComparisonItemDto
            {
                Id = vendor.Id,
                Name = vendor.Name,
                OverallRiskScore = assessment.RiskScore,
                RiskLevel = assessment.RiskLevel.ToString(),
                Rank = 0,
                Strengths = strengths,
                Weaknesses = weaknesses
            });
        }

        vendorComparisons = vendorComparisons
            .OrderBy(v => v.OverallRiskScore)
            .Select((v, index) => v with { Rank = index + 1 })
            .ToList();

        var summary = CreateSummary(vendorComparisons);
        var recommendation = GenerateRecommendation(vendorComparisons);

        return new CompareVendorsQueryResponse
        {
            Comparison = new VendorComparisonDto
            {
                Vendors = vendorComparisons,
                Summary = summary,
                Recommendation = recommendation
            }
        };
    }

    private List<string> IdentifyStrengths(Domain.Entities.VendorProfile vendor, Domain.Entities.RiskAssessment assessment)
    {
        var strengths = new List<string>();

        if (vendor.FinancialHealth >= 80)
            strengths.Add($"Strong financial health ({vendor.FinancialHealth}/100)");

        if (vendor.SlaUptime >= 99)
            strengths.Add($"Excellent SLA uptime ({vendor.SlaUptime}%)");

        if (vendor.MajorIncidents == 0)
            strengths.Add("Zero major incidents");

        if (vendor.SecurityCerts.Count >= 2)
            strengths.Add($"{vendor.SecurityCerts.Count} security certifications");

        if (vendor.Documents.ContractValid &&
            vendor.Documents.PrivacyPolicyValid &&
            vendor.Documents.PentestReportValid)
            strengths.Add("All compliance documents valid");

        if (assessment.RiskScore <= 0.25m)
            strengths.Add("Low overall risk profile");

        return strengths;
    }

    private List<string> IdentifyWeaknesses(Domain.Entities.VendorProfile vendor, Domain.Entities.RiskAssessment assessment)
    {
        var weaknesses = new List<string>();

        if (vendor.FinancialHealth < 50)
            weaknesses.Add($"Poor financial health ({vendor.FinancialHealth}/100)");

        if (vendor.SlaUptime < 90)
            weaknesses.Add($"Low SLA uptime ({vendor.SlaUptime}%)");

        if (vendor.MajorIncidents >= 3)
            weaknesses.Add($"High incident count ({vendor.MajorIncidents})");

        if (vendor.SecurityCerts.Count == 0)
            weaknesses.Add("No security certifications");

        if (!vendor.Documents.ContractValid)
            weaknesses.Add("Contract expired or invalid");

        if (!vendor.Documents.PrivacyPolicyValid)
            weaknesses.Add("Privacy policy expired or invalid");

        if (assessment.RiskScore >= 0.75m)
            weaknesses.Add("Critical risk level");

        return weaknesses;
    }

    private ComparisonSummaryDto CreateSummary(List<VendorRiskComparisonItemDto> vendors)
    {
        if (!vendors.Any())
        {
            return new ComparisonSummaryDto();
        }

        var lowestRisk = vendors.OrderBy(v => v.OverallRiskScore).First();
        var highestRisk = vendors.OrderByDescending(v => v.OverallRiskScore).First();
        var avgRisk = vendors.Average(v => v.OverallRiskScore);

        var distribution = vendors
            .GroupBy(v => v.RiskLevel)
            .ToDictionary(g => g.Key, g => g.Count());

        var commonRisks = vendors
            .SelectMany(v => v.Weaknesses)
            .GroupBy(w => w)
            .Where(g => g.Count() >= vendors.Count / 2)
            .Select(g => g.Key)
            .ToList();

        return new ComparisonSummaryDto
        {
            LowestRiskVendor = lowestRisk.Name,
            HighestRiskVendor = highestRisk.Name,
            AverageRiskScore = avgRisk,
            RiskLevelDistribution = distribution,
            CommonRisks = commonRisks
        };
    }

    private string GenerateRecommendation(List<VendorRiskComparisonItemDto> vendors)
    {
        if (!vendors.Any()) return "No vendors to compare";

        var best = vendors.First();
        var worst = vendors.Last();

        var recommendation = $"Recommendation:\n\n";
        recommendation += $"✓ Primary Choice: {best.Name} (Rank #{best.Rank}, {best.RiskLevel} Risk)\n";
        recommendation += $"  Overall Risk Score: {best.OverallRiskScore:P2}\n";

        if (best.Strengths.Any())
        {
            recommendation += $"  Key Strengths:\n";
            foreach (var strength in best.Strengths.Take(3))
            {
                recommendation += $"    • {strength}\n";
            }
        }

        recommendation += $"\n✗ Highest Risk: {worst.Name} (Rank #{worst.Rank}, {worst.RiskLevel} Risk)\n";
        recommendation += $"  Overall Risk Score: {worst.OverallRiskScore:P2}\n";

        if (worst.Weaknesses.Any())
        {
            recommendation += $"  Critical Issues:\n";
            foreach (var weakness in worst.Weaknesses.Take(3))
            {
                recommendation += $"    • {weakness}\n";
            }
        }

        var scoreDiff = worst.OverallRiskScore - best.OverallRiskScore;
        recommendation += $"\nRisk Differential: {scoreDiff:P2} ({best.Name} is significantly safer)\n";

        return recommendation;
    }
}
