namespace UpStatus.Infrastructure.Caching;

public sealed class RedisOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "upstatus";
}
