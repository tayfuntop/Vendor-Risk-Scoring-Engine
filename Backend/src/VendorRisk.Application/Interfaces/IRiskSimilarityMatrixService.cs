namespace VendorRisk.Application.Interfaces;

public interface IRiskSimilarityMatrixService
{
    Dictionary<string, decimal> GetCorrelatedRisks(List<string> riskFactors);
    decimal GetSimilarityScore(string risk1, string risk2);
}
