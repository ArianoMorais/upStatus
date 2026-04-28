using FluentValidation;
using UpStatus.Api.Contracts.Requests.Monitors;

namespace UpStatus.Application.Validators.Monitors;

public sealed class MonitorConfigValidator : AbstractValidator<MonitorConfigRequest>
{
    private static readonly string[] AllowedMethods = ["GET", "HEAD", "POST"];

    public MonitorConfigValidator()
    {
        RuleFor(x => x.IntervalSeconds)
            .InclusiveBetween(10, 3600).WithMessage("Intervalo deve estar entre 10 e 3600 segundos.");

        RuleFor(x => x.TimeoutMs)
            .InclusiveBetween(500, 60000).WithMessage("Timeout deve estar entre 500 e 60000 ms.");

        RuleFor(x => x.FailuresToOpenIncident)
            .InclusiveBetween(1, 20).WithMessage("Falhas para abrir incidente deve estar entre 1 e 20.");

        RuleFor(x => x.SuccessesToCloseIncident)
            .InclusiveBetween(1, 20).WithMessage("Sucessos para fechar incidente deve estar entre 1 e 20.");

        RuleFor(x => x.DegradedLatencyMs)
            .InclusiveBetween(100, 60000).WithMessage("Latência degradada deve estar entre 100 e 60000 ms.");

        RuleFor(x => x.ExpectedStatusCode)
            .InclusiveBetween(100, 599).When(x => x.ExpectedStatusCode.HasValue)
            .WithMessage("Status code esperado deve estar entre 100 e 599.");

        RuleFor(x => x.HttpMethod)
            .NotEmpty().WithMessage("Método HTTP é obrigatório.")
            .Must(m => AllowedMethods.Contains(m.ToUpperInvariant()))
            .WithMessage("Método HTTP deve ser GET, HEAD ou POST.");
    }
}
