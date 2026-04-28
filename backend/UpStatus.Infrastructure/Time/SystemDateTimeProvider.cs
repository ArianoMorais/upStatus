using UpStatus.Application.Common.Abstractions;

namespace UpStatus.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.Now;
}
