using FastEndpoints;
using FluentValidation;
using UpStatus.Api.Contracts.Requests.Monitors;

namespace UpStatus.Application.Validators.Monitors;

public sealed class UpdateMonitorValidator : Validator<UpdateMonitorRequest>
{
    public UpdateMonitorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(120).WithMessage("Nome deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL é obrigatória.")
            .Must(BeValidHttpUrl).WithMessage("URL deve ser http(s) absoluta e válida.");

        RuleFor(x => x.Config)
            .NotNull().WithMessage("Configuração é obrigatória.")
            .SetValidator(new MonitorConfigValidator());
    }

    private static bool BeValidHttpUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed))
        {
            return false;
        }

        return parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps;
    }
}
