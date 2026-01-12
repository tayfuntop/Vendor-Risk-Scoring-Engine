namespace VendorRisk.Infrastructure.Configuration;

public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
    public int DefaultExpirationMinutes { get; set; } = 5;
}
