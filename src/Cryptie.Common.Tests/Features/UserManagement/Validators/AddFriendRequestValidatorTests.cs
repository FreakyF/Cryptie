using System;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Common.Features.UserManagement.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Cryptie.Common.Tests.Features.UserManagement.Validators;

public class AddFriendRequestValidatorTests
{
    private readonly AddFriendRequestValidator _validator = new();

    [Theory]
    [InlineData(null, "Username cannot be empty")]
    [InlineData("", "Username cannot be empty")]
    [InlineData(" ", "Username cannot be empty")]
    [InlineData("abc$%", "Username contains invalid characters")]
    [InlineData("abcd", "Username must be at least 5 characters long")]
    [InlineData("abcdefghijklmnopqrstu", "Username cannot exceed 20 characters")]
    public void InvalidFriend_ShouldHaveValidationError(string friend, string expectedError)
    {
        var model = new AddFriendRequestDto
        {
            Friend = friend,
            SessionToken = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Friend).WithErrorMessage(expectedError);
    }

    [Fact]
    public void ValidFriend_ShouldNotHaveValidationError()
    {
        var model = new AddFriendRequestDto
        {
            Friend = "ValidUser123",
            SessionToken = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Friend);
    }

    [Theory]
    [InlineData(null, "Session token is required")]
    public void NullSessionToken_ShouldHaveValidationError(Guid? sessionToken, string expectedError)
    {
        var model = new AddFriendRequestDto
        {
            Friend = "ValidUser123",
            SessionToken = sessionToken ?? Guid.Empty
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SessionToken).WithErrorMessage(expectedError);
    }

    [Fact]
    public void EmptySessionToken_ShouldHaveValidationError()
    {
        var model = new AddFriendRequestDto
        {
            Friend = "ValidUser123",
            SessionToken = Guid.Empty
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SessionToken).WithErrorMessage("Session token must be a valid GUID");
    }

    [Fact]
    public void ValidSessionToken_ShouldNotHaveValidationError()
    {
        var model = new AddFriendRequestDto
        {
            Friend = "ValidUser123",
            SessionToken = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.SessionToken);
    }

    [Fact]
    public void ValidModel_ShouldNotHaveAnyValidationErrors()
    {
        var model = new AddFriendRequestDto
        {
            Friend = "ValidUser123",
            SessionToken = Guid.NewGuid()
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

