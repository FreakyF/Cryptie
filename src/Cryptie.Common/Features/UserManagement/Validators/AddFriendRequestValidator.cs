using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;

namespace Cryptie.Common.Features.UserManagement.Validators;

public class AddFriendRequestValidator : AbstractValidator<AddFriendRequestDto>
{
    public AddFriendRequestValidator()
    {
        RuleFor(x => x.Friend)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username cannot be empty")
            .Matches("^[A-Za-z0-9]+$").WithMessage("Username contains invalid characters")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long")
            .MaximumLength(20).WithMessage("Username cannot exceed 20 characters");

        RuleFor(x => x.SessionToken)
            .NotEmpty().WithMessage("Session token is required")
            .Must(token => token != Guid.Empty)
            .WithMessage("Session token must be a valid GUID");
    }
}