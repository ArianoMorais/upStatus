using UpStatus.Api.Contracts.Responses.Checks;
using UpStatus.Domain.Checks;

namespace UpStatus.Application.Common.Mappings;

public static class CheckMappings
{
    public static CheckResponse ToCheckResponse(this Check check) => new()
    {
        Id = check.Id,
        MonitorId = check.MonitorId,
        Timestamp = check.Timestamp,
        Result = check.Result.ToString(),
        StatusCode = check.StatusCode,
        LatencyMs = check.LatencyMs,
        ErrorMessage = check.ErrorMessage
    };
}
