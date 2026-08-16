using FluentValidation;

namespace Custodian.Application.Features.Invitations.InviteCompany.Commands;

public class InviteCompanyCommandValidator : AbstractValidator<InviteCompanyCommand>
{
    public InviteCompanyCommandValidator()
    {
        RuleFor(v => v.email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
            .EmailAddress().WithMessage("Must be a valid email address.");
    }
}
