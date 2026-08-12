using Custodian.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Custodian.Application.Features.Invitations.AcceptInvitation.Commands;

public class AcceptInvitationCommandValidator: AbstractValidator<AccepInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(256).WithMessage("Name can not exceed 256 characters.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("The invitation link is invalid or corrupted.Please click the link in your email again.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
    }
}
