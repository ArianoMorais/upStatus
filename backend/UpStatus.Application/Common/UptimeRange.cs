namespace UpStatus.Application.Common;

public static class UptimeRange
{
    public static bool TryParse(string? value, out TimeSpan duration, out string normalized)
    {
        normalized = string.IsNullOrWhiteSpace(value) ? "24h" : value.Trim().ToLowerInvariant();

        duration = normalized switch
        {
            "1h" => TimeSpan.FromHours(1),
            "24h" => TimeSpan.FromHours(24),
            "7d" => TimeSpan.FromDays(7),
            "30d" => TimeSpan.FromDays(30),
            _ => TimeSpan.Zero
        };

        return duration > TimeSpan.Zero;
    }
}
