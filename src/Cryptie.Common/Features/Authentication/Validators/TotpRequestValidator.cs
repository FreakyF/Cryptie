using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;

namespace Cryptie.Common.Features.Authentication.Validators;

public class TotpRequestValidator : AbstractValidator<TotpRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TotpRequestValidator"/> class
    /// and establishes validation rules for two-factor authentication requests.
    /// </summary>
    public TotpRequestValidator()
    {
        RuleFor(x => x.TotpToken)
            .NotEmpty().WithMessage("Token cannot be empty.");

        RuleFor(x => x.Secret)
            .NotEmpty()
            .WithMessage("Authentication code cannot be empty.")
            .Length(6)
            .WithMessage("Authentication code must be 6 digits long.")
            .Matches(@"^\d+$")
            .WithMessage("Authentication code contains invalid characters.");
    }
}