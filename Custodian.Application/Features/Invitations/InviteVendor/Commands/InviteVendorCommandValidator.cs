using FluentValidation;
namespace Custodian.Application.Features.Invitations.InviteVendor.Commands;

public class InviteVendorCommandValidator: AbstractValidator<InviteVendorCommand>
{
    public InviteVendorCommandValidator()
    {
        RuleFor(v => v.email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
            .EmailAddress().WithMessage("Must be a valid email address.");

        RuleFor(v => v.invitedById)
            .NotEmpty().WithMessage("InvitedBy ID is required.");
    }
}
