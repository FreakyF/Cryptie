using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using Moq;
using Moq.Protected;
using Xunit;

public class AuthenticationServiceTests
{
    private HttpClient CreateHttpClient(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new System.Uri("https://fakeapi.com/")
        };
    }
    [Fact]
    public async Task RegisterAsync_ReturnsResponseDto()
    {
        // Arrange – Tworzymy symulowaną odpowiedź z serwera
        var expected = new RegisterResponseDto
        {
            Secret = "mock-secret",
            TotpToken = Guid.Empty,
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        };

        var httpClient = CreateHttpClient(response);
        var service = new AuthenticationService(httpClient);

        var request = new RegisterRequestDto
        {
            Login = "testuser",
            Password = "P@ssw0rd",
            DisplayName = "Test User",
            Email = "test@example.com"
        };

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mock-secret", result!.Secret);
    }

}
