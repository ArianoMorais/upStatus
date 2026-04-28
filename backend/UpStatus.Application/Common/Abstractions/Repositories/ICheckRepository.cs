using UpStatus.Domain.Checks;

namespace UpStatus.Application.Common.Abstractions.Repositories;

public interface ICheckRepository
{
    Task AddAsync(Check check, CancellationToken ct);

    Task<IReadOnlyList<Check>> ListByMonitorAsync(Guid monitorId, DateTime from, DateTime to, CancellationToken ct);
}
