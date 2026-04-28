using FastEndpoints;
using UpStatus.Api.Common;
using UpStatus.Api.Contracts.Requests.Monitors;
using UpStatus.Api.Contracts.Responses.Monitors;
using UpStatus.Application.Features.Monitors.Create;

namespace UpStatus.Api.Endpoints.Monitors;

public sealed class CreateMonitorEndpoint : Endpoint<CreateMonitorRequest, CreateMonitorResponse>
{
    public override void Configure()
    {
        Post("/monitors");
        Description(b => b.WithTags("Monitors"));
    }

    public override async Task HandleAsync(CreateMonitorRequest req, CancellationToken ct)
    {
        var userId = CurrentUserAccessor.GetUserId(User);

        var command = new CreateMonitorCommand(req.Name, req.Url, req.Config, userId);
        var response = await command.ExecuteAsync(ct);

        await Send.CreatedAtAsync<GetMonitorByIdEndpoint>(
            new { id = response.Id },
            response,
            generateAbsoluteUrl: false,
            cancellation: ct);
    }
}
