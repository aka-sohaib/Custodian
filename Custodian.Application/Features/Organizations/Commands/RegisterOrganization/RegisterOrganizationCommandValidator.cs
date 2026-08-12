using FluentValidation;

namespace Custodian.Application.Features.Organizations.Commands.RegisterOrganization;

public class RegisterOrganizationCommandValidator : AbstractValidator<RegisterOrganizationCommand>
{
    public RegisterOrganizationCommandValidator()
    {
        //---- Organization Rules ----
        RuleFor(x => x.OrganizationName)
            .NotEmpty().WithMessage("Organization Name is required.")
            .MaximumLength(200).WithMessage("Organization Name cannot exceed 200 characters.");

        RuleFor(x => x.OrganizationEmail)
            .NotEmpty().WithMessage("Organization Email is required.")
            .MaximumLength(256).WithMessage("Organization Email cannot exceed 256 characters.")
            .EmailAddress().WithMessage("A valid Organization Email is required.");

        RuleFor(x => x.OrganizationPhone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in valid international format (e.g., +923121234567).");

        RuleFor(x => x)
            .Must(x => x.IsCompany || x.IsVendor)
            .WithMessage("Organization must be registered as a Company, a Vendor, or both.");

        //---- Admin Rules ----
        RuleFor(x => x.AdminName)
            .NotEmpty().WithMessage("Admin Name is required.")
            .MaximumLength(256);

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Admin Email is required.")
            .MaximumLength(256).WithMessage("Admin Email cannot exceed 256 characters.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
    }
}
