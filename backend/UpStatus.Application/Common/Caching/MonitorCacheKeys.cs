namespace UpStatus.Application.Common.Caching;

public static class MonitorCacheKeys
{
    public static string Status(Guid monitorId) => $"monitor:status:{monitorId}";
}
