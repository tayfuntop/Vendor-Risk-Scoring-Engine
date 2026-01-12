namespace VendorRisk.Infrastructure.Configuration;

public class RiskEngineSettings
{
    public string MatrixFilePath { get; set; } = string.Empty;
    public WeightSettings Weights { get; set; } = new();
    public decimal CorrelationThreshold { get; set; } = 0.7m;
}

public class WeightSettings
{
    public decimal Financial { get; set; } = 0.4m;
    public decimal Operational { get; set; } = 0.3m;
    public decimal Security { get; set; } = 0.3m;
}
