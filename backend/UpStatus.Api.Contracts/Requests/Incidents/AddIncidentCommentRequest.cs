namespace UpStatus.Api.Contracts.Requests.Incidents;

public sealed record AddIncidentCommentRequest
{
    public string Message { get; init; } = string.Empty;
}
