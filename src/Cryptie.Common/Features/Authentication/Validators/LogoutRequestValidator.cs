﻿using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;

namespace Cryptie.Common.Features.Authentication.Validators;

public class LogoutRequestValidator : AbstractValidator<LogoutRequestDto>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}