namespace UpStatus.Api.Contracts.Requests.Incidents;

public sealed record ResolveIncidentRequest
{
    public string? Comment { get; init; }
}
