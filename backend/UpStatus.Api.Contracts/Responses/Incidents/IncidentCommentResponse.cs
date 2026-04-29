namespace UpStatus.Api.Contracts.Responses.Incidents;

public sealed record IncidentCommentResponse
{
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
