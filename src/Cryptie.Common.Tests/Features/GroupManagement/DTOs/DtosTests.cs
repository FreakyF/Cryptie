using Cryptie.Common.Features.GroupManagement;

namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;

public class DtosTests
{
    [Trait("TestCategory", "Unit")]
    [Fact]
    public void AddUserToGroupRequestDTO_ShouldSetAndReturnCorrectValue()
    {
        var expectedGroupGuid = Guid.NewGuid();
        var expectedSessionToken = Guid.NewGuid();
        var expectedUserToAdd = Guid.NewGuid();

        var dto = new AddUserToGroupRequestDto
        {
            GroupGuid = expectedGroupGuid,
            SessionToken = expectedSessionToken,
            UserToAdd = expectedUserToAdd
        };

        Assert.Equal(expectedGroupGuid, dto.GroupGuid);
        Assert.Equal(expectedSessionToken, dto.SessionToken);
        Assert.Equal(expectedUserToAdd, dto.UserToAdd);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("name")]
    public void ChangeGroupNameRequestDto_ShouldSetAndReturnCorrectValue(string name)
    {
        var expectedGroupGuid = Guid.NewGuid();
        var expectedSessionToken = Guid.NewGuid();

        var dto = new ChangeGroupNameRequestDto
        {
            GroupGuid = expectedGroupGuid,
            NewName = name,
            SessionToken = expectedSessionToken
        };

        Assert.Equal(expectedGroupGuid, dto.GroupGuid);
        Assert.Equal(name, dto.NewName);
        Assert.Equal(expectedSessionToken, dto.SessionToken);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("name")]
    public void CreateGroupRequestDTO_ShouldSetAndReturnCorrectValue(string name)
    {
        var expectedSessionToken = Guid.NewGuid();

        var dto = new CreateGroupRequestDto
        {
            Name = name,
            SessionToken = expectedSessionToken
        };

        Assert.Equal(name, dto.Name);
        Assert.Equal(expectedSessionToken, dto.SessionToken);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void CreateGroupResponseDTO_ShouldSetAndReturnCorrectValue()
    {
        var expectedGroupGuid = Guid.NewGuid();

        var dto = new CreateGroupResponseDto
        {
            Group = expectedGroupGuid
        };

        Assert.Equal(expectedGroupGuid, dto.Group);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void DeleteGroupRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();
        var expectedGroupGuid = Guid.NewGuid();

        var dto = new DeleteGroupRequestDto
        {
            SessionToken = expectedSessionToken,
            GroupGuid = expectedGroupGuid
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
        Assert.Equal(expectedGroupGuid, dto.GroupGuid);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void RemoveUserFromGroupRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedSessionToken = Guid.NewGuid();
        var expectedGroupGuid = Guid.NewGuid();
        var expectedUserToRemove = Guid.NewGuid();

        var dto = new RemoveUserFromGroupRequestDto
        {
            SessionToken = expectedSessionToken,
            GroupGuid = expectedGroupGuid,
            UserToRemove = expectedUserToRemove
        };

        Assert.Equal(expectedSessionToken, dto.SessionToken);
        Assert.Equal(expectedGroupGuid, dto.GroupGuid);
        Assert.Equal(expectedUserToRemove, dto.UserToRemove);
    }
}