using Cryptie.Client.Features.Authentication.State;
using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Tests.Features.Authentication.State
{
    public class RegistrationStateTests
    {
        [Fact]
        public void LastResponse_ShouldBeNull_ByDefault()
        {
            // Arrange
            var registrationState = new RegistrationState();

            // Act & Assert
            Assert.Null(registrationState.LastResponse);
        }

        [Fact]
        public void LastResponse_ShouldSetAndGetValue()
        {
            // Arrange
            var registrationState = new RegistrationState();
            var response = new RegisterResponseDto { Secret = "dummy-secret", TotpToken = Guid.NewGuid() };

            // Act
            registrationState.LastResponse = response;

            // Assert
            Assert.Equal(response, registrationState.LastResponse);
        }
    }
}
