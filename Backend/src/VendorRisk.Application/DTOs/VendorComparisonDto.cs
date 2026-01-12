namespace VendorRisk.Application.DTOs;

public record VendorComparisonDto
{
    public List<VendorRiskComparisonItemDto> Vendors { get; init; } = new();
    public ComparisonSummaryDto Summary { get; init; } = null!;
    public string Recommendation { get; init; } = string.Empty;
}

public record VendorRiskComparisonItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal OverallRiskScore { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public int Rank { get; init; }
    public List<string> Strengths { get; init; } = new();
    public List<string> Weaknesses { get; init; } = new();
}

public record ComparisonSummaryDto
{
    public string LowestRiskVendor { get; init; } = string.Empty;
    public string HighestRiskVendor { get; init; } = string.Empty;
    public decimal AverageRiskScore { get; init; }
    public Dictionary<string, int> RiskLevelDistribution { get; init; } = new();
    public List<string> CommonRisks { get; init; } = new();
}
