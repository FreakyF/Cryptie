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
}