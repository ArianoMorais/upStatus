namespace UpStatus.Domain.Incidents;

public sealed class IncidentComment
{
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
