using FastEndpoints;
using UpStatus.Api.Contracts.Responses.Auth;

namespace UpStatus.Application.Features.Auth.GetCurrentUser;

public sealed record GetCurrentUserQuery : ICommand<AuthUserResponse>
{
    public Guid UserId { get; }

    public GetCurrentUserQuery(Guid userId)
    {
        UserId = userId;
    }
}
