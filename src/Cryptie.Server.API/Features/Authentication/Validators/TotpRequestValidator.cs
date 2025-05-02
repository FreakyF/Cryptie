using Cryptie.Server.Domain.Features.Authentication.DTOs;
using FluentValidation;

namespace Cryptie.Server.API.Features.Authentication.Validators;

public class TotpRequestValidator : AbstractValidator<TotpRequest>
{
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