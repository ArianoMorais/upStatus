using FastEndpoints;
using FluentValidation;
using UpStatus.Api.Contracts.Requests.Incidents;

namespace UpStatus.Application.Validators.Incidents;

public sealed class AddIncidentCommentValidator : Validator<AddIncidentCommentRequest>
{
    public AddIncidentCommentValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Mensagem é obrigatória.")
            .MaximumLength(2000).WithMessage("Mensagem deve ter no máximo 2000 caracteres.");
    }
}
