using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;

namespace Cryptie.Common.Features.Authentication.Validators;

public class LogoutRequestValidator : AbstractValidator<LogoutRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutRequestValidator"/> class
    /// and configures validation rules for user logout.
    /// </summary>
    public LogoutRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}