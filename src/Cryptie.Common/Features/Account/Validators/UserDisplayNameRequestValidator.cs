using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;

namespace Cryptie.Common.Features.Account.Validators;

public class UserDisplayNameRequestValidator : AbstractValidator<UserDisplayNameRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDisplayNameRequestValidator"/> class
    /// and defines validation rules for user display name changes.
    /// </summary>
    public UserDisplayNameRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .Must(v => !string.IsNullOrWhiteSpace(v)).WithMessage("Username cannot be empty")
            .Matches("^[A-Za-z0-9]+$").WithMessage("Username contains invalid characters")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long")
            .MaximumLength(20).WithMessage("Username cannot exceed 20 characters");
    }
}