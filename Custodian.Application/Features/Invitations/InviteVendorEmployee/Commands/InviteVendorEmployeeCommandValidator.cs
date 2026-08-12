using FluentValidation;

namespace Custodian.Application.Features.Invitations.InviteVendorEmployee.Commands;

public class InviteVendorEmployeeCommandValidator: AbstractValidator<InviteVendorEmployeeCommand>
{
    public InviteVendorEmployeeCommandValidator()
    {
        RuleFor(v => v.email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
            .EmailAddress().WithMessage("Must be a valid email address.");

        RuleFor(v => v.role)
            .NotEmpty().WithMessage("Role is required.")
            .IsInEnum().WithMessage("A valid employee role must be selected.");

        RuleFor(v => v.invitedById)
            .NotEmpty().WithMessage("InvitedBy ID is required.");
    }
}