using Cryptie.Client.Features.Authentication.State;
using Cryptie.Common.Features.Authentication.DTOs;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.State
{
    public class LoginStateTests
    {
        [Fact]
        public void LastResponse_ShouldBeNull_ByDefault()
        {
            // Arrange
            var loginState = new LoginState();

            // Act & Assert
            Assert.Null(loginState.LastResponse);
        }

        [Fact]
        public void LastResponse_ShouldSetAndGetValue()
        {
            // Arrange
            var loginState = new LoginState();
            var response = new LoginResponseDto { TotpToken = Guid.NewGuid() };

            // Act
            loginState.LastResponse = response;

            // Assert
            Assert.Equal(response, loginState.LastResponse);
        }
    }
}
