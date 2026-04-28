using System.Security.Claims;
using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Auth;
using UpStatus.Application.Features.Auth.GetCurrentUser;

namespace UpStatus.Api.Endpoints.Auth;

public sealed class GetMeEndpoint : EndpointWithoutRequest<AuthUserResponse>
{
    public override void Configure()
    {
        Get("/auth/me");
        Description(b => b.WithTags("Auth"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var query = new GetCurrentUserQuery(userId);
        var response = await query.ExecuteAsync(ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
