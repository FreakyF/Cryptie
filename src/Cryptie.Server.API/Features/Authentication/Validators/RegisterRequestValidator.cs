using Cryptie.Server.Domain.Features.Authentication.DTOs;
using FluentValidation;

namespace Cryptie.Server.API.Features.Authentication.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Username cannot be empty")
            .Matches("^[A-Za-z0-9]+$").WithMessage("Username contains invalid characters")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long")
            .MaximumLength(20).WithMessage("Username cannot exceed 20 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .Matches(@"^\P{C}+$").WithMessage("Password contains invalid characters")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit")
            .Matches("[^A-Za-z0-9]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("First name cannot be empty")
            .Matches(@"^[\p{L} \-'.]+$").WithMessage("First name contains invalid characters")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .Matches(@"^[\p{L} \-'.]+$").WithMessage("Last name contains invalid characters")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Email address is invalid");
    }
}