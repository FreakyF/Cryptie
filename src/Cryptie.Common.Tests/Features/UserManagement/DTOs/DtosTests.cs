using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Common.Tests.Features.UserManagement.DTOs;

public class DtosTests
{
    [Trait("TestCategory", "Unit")]
    [Fact]
    public void AddFriendRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();
        var expectedFriend = "f";

        var dto = new AddFriendRequestDto
        {
            SessionToken = expectedSessionToken,
            Friend = expectedFriend
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
        Assert.Equal(expectedFriend, dto.Friend);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void FriendListRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();

        var dto = new FriendListRequestDto
        {
            SessionToken = expectedSessionToken
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void GetFriendListResponseDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedFriend1 = Guid.NewGuid();
        var expectedFriend2 = Guid.NewGuid();
        var expectedFriend3 = Guid.NewGuid();

        var dto = new GetFriendListResponseDto
        {
            Friends = [expectedFriend1, expectedFriend2, expectedFriend3]
        };

        Assert.Contains(expectedFriend1, dto.Friends);
        Assert.Contains(expectedFriend2, dto.Friends);
        Assert.Contains(expectedFriend3, dto.Friends);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void NameFromGuidRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedUserGuid = Guid.NewGuid();

        var dto = new NameFromGuidRequestDto
        {
            Id = expectedUserGuid
        };

        Assert.Equal(expectedUserGuid, dto.Id);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("name")]
    public void NameFromGuidResponseDto_ShouldSetAndReturnCorrectValue(string name)
    {
        var dto = new NameFromGuidResponseDto
        {
            Name = name
        };

        Assert.Equal(name, dto.Name);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("name")]
    public void UserDisplayNameRequestDto_ShouldSetAndReturnCorrectValue(string name)
    {
        var expectedSessionToken = Guid.NewGuid();

        var dto = new UserDisplayNameRequestDto
        {
            Name = name,
            Token = expectedSessionToken
        };

        Assert.Equal(name, dto.Name);
        Assert.Equal(expectedSessionToken, dto.Token);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void UserGroupsRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();

        var dto = new UserGroupsRequestDto
        {
            SessionToken = expectedSessionToken
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void UserGroupsResponseDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedGroupGuid1 = Guid.NewGuid();
        var expectedGroupGuid2 = Guid.NewGuid();
        var expectedGroupGuid3 = Guid.NewGuid();

        var dto = new UserGroupsResponseDto
        {
            Groups = [expectedGroupGuid1, expectedGroupGuid2, expectedGroupGuid3]
        };

        Assert.Contains(expectedGroupGuid1, dto.Groups);
        Assert.Contains(expectedGroupGuid2, dto.Groups);
        Assert.Contains(expectedGroupGuid3, dto.Groups);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void UserGuidFromTokenRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();

        var dto = new UserGuidFromTokenRequestDto
        {
            SessionToken = expectedSessionToken
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void UserLoginFromTokenRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();

        var dto = new UserLoginFromTokenRequestDto
        {
            SessionToken = expectedSessionToken
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("login")]
    public void UserLoginFromTokenResponseDto_ShouldSetAndReturnCorrectValue(string login)
    {
        var dto = new UserLoginFromTokenResponseDto
        {
            Login = login
        };

        Assert.Equal(login, dto.Login);
    }
}