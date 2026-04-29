using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using UpStatus.Api.Contracts.Responses.Incidents;
using UpStatus.Application.Features.Checks.Record;
using UpStatus.Application.Features.Incidents.Open;
using UpStatus.Application.Features.Incidents.Resolve;

namespace UpStatus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RecordCheckCommandHandler>();
        services.AddScoped<ICommandHandler<RecordCheckCommand, EmptyResponse>>(
            sp => sp.GetRequiredService<RecordCheckCommandHandler>());

        services.AddScoped<OpenIncidentCommandHandler>();
        services.AddScoped<ICommandHandler<OpenIncidentCommand, IncidentResponse>>(
            sp => sp.GetRequiredService<OpenIncidentCommandHandler>());

        services.AddScoped<ResolveIncidentCommandHandler>();
        services.AddScoped<ICommandHandler<ResolveIncidentCommand, IncidentResponse>>(
            sp => sp.GetRequiredService<ResolveIncidentCommandHandler>());

        return services;
    }
}
