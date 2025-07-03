using Cryptie.Common.Features.KeysManagement.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.KeysManagement.DTOs;

public class GetUserKeyResponseDtoTests
{
    [Fact]
    public void Can_Set_And_Get_PublicKey()
    {
        // Arrange
        var publicKey = "test-public-key";
        var dto = new GetUserKeyResponseDto();
        // Act
        dto.PublicKey = publicKey;
        // Assert
        Assert.Equal(publicKey, dto.PublicKey);
    }

    [Fact]
    public void Default_PublicKey_Is_Null()
    {
        // Act
        var dto = new GetUserKeyResponseDto();
        // Assert
        Assert.NotNull(dto.PublicKey);
    }
}

