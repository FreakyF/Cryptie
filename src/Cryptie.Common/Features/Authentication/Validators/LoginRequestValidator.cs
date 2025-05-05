using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;

namespace Cryptie.Server.API.Features.Authentication.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("Username cannot be empty")
            .Matches("^[A-Za-z0-9]+$")
            .WithMessage("Username contains invalid characters")
            .MinimumLength(5)
            .WithMessage("Username must be at least 5 characters long")
            .MaximumLength(20)
            .WithMessage("Username cannot exceed 20 characters");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v))
            .WithMessage("Password cannot be empty")
            .Matches(@"^\P{C}+$")
            .WithMessage("Password contains invalid characters")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(20)
            .WithMessage("Password cannot exceed 20 characters")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches("[^A-Za-z0-9]")
            .WithMessage("Password must contain at least one special character");
    }
}