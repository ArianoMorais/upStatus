namespace UpStatus.Application.Common.Caching;

public static class IncidentCacheKeys
{
    public static string Cooldown(Guid monitorId) => $"alert:cooldown:{monitorId}";
}
