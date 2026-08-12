using FluentValidation;

namespace Custodian.Application.Features.Users.Login;

public class LoginCommandValidator: AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(256).WithMessage("Email length should not exceed 256 characters")
            .EmailAddress().WithMessage("Email Address is invalid.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(500).WithMessage("Password Length can not exceed 500 characters");
    }
}
