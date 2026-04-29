using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Domain.Incidents;

namespace UpStatus.Application.Common.Mappings;

public static class IncidentMappings
{
    public static IncidentResponse ToIncidentResponse(this Incident incident) => new()
    {
        Id = incident.Id,
        MonitorId = incident.MonitorId,
        Status = incident.Status.ToString(),
        StartedAt = incident.StartedAt,
        AcknowledgedAt = incident.AcknowledgedAt,
        ResolvedAt = incident.ResolvedAt,
        AcknowledgedBy = incident.AcknowledgedBy,
        Reason = incident.Reason,
        Comments = incident.Comments.Select(c => c.ToIncidentCommentResponse()).ToList()
    };

    public static IncidentCommentResponse ToIncidentCommentResponse(this IncidentComment comment) => new()
    {
        AuthorId = comment.AuthorId,
        AuthorName = comment.AuthorName,
        Message = comment.Message,
        CreatedAt = comment.CreatedAt
    };
}
