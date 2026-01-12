using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VendorRisk.Application.Interfaces;

namespace VendorRisk.Infrastructure.Services;

public class RiskSimilarityMatrixService : IRiskSimilarityMatrixService
{
    private readonly Dictionary<string, Dictionary<string, decimal>> _similarityMatrix;
    private readonly ILogger<RiskSimilarityMatrixService> _logger;

    public RiskSimilarityMatrixService(string matrixFilePath, ILogger<RiskSimilarityMatrixService> logger)
    {
        _logger = logger;
        _similarityMatrix = new Dictionary<string, Dictionary<string, decimal>>();
        LoadMatrix(matrixFilePath);
    }

    private void LoadMatrix(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Risk similarity matrix file not found at {FilePath}", filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var jObject = JObject.Parse(json);

            foreach (var category in jObject)
            {
                if (category.Value is JObject categoryObj)
                {
                    foreach (var riskFactor in categoryObj)
                    {
                        var factorName = riskFactor.Key;
                        if (riskFactor.Value is JObject similarities)
                        {
                            if (!_similarityMatrix.ContainsKey(factorName))
                            {
                                _similarityMatrix[factorName] = new Dictionary<string, decimal>();
                            }

                            foreach (var similarity in similarities)
                            {
                                var relatedFactor = similarity.Key;
                                var score = similarity.Value?.Value<decimal>() ?? 0m;
                                _similarityMatrix[factorName][relatedFactor] = score;
                            }
                        }
                    }
                }
            }

            _logger.LogInformation("Loaded {Count} risk factors from similarity matrix", _similarityMatrix.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load risk similarity matrix from {FilePath}", filePath);
        }
    }

    public Dictionary<string, decimal> GetCorrelatedRisks(List<string> riskFactors)
    {
        var correlatedRisks = new Dictionary<string, decimal>();

        foreach (var riskFactor in riskFactors)
        {
            if (_similarityMatrix.ContainsKey(riskFactor))
            {
                foreach (var similarity in _similarityMatrix[riskFactor])
                {
                    if (similarity.Value >= 0.7m)
                    {
                        if (!correlatedRisks.ContainsKey(similarity.Key) || correlatedRisks[similarity.Key] < similarity.Value)
                        {
                            correlatedRisks[similarity.Key] = similarity.Value;
                        }
                    }
                }
            }
        }

        return correlatedRisks;
    }

    public decimal GetSimilarityScore(string risk1, string risk2)
    {
        if (_similarityMatrix.ContainsKey(risk1) && _similarityMatrix[risk1].ContainsKey(risk2))
        {
            return _similarityMatrix[risk1][risk2];
        }

        if (_similarityMatrix.ContainsKey(risk2) && _similarityMatrix[risk2].ContainsKey(risk1))
        {
            return _similarityMatrix[risk2][risk1];
        }

        return 0m;
    }
}
