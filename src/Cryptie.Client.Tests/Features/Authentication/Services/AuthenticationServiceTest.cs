/*using System;
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

namespace Cryptie.Client.Tests.Features.Authentication.Services;

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
            BaseAddress = new Uri("https://fakeapi.com/")
        };
    }

    [Fact]
    public async Task RegisterAsync_ReturnsResponseDto()
    {
        var expected = new RegisterResponseDto
        {
            Secret = "mySecret",
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
            Password = "securepass",
            DisplayName = "Test User",
            Email = "test@example.com",
            PrivateKey = "privatekey",
            PublicKey = "publickey"
        };

        var result = await service.RegisterAsync(request);

        Assert.NotNull(result);
        Assert.Equal("mySecret", result!.Secret);
    }

    [Fact]
    public async Task LoginAsync_ReturnsResponseDto()
    {
        var expectedToken = Guid.NewGuid();

        var expected = new LoginResponseDto
        {
            TotpToken = expectedToken
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        };

        var httpClient = CreateHttpClient(response);
        var service = new AuthenticationService(httpClient);

        var request = new LoginRequestDto
        {
            Login = "testuser",
            Password = "securepass"
        };

        var result = await service.LoginAsync(request);

        Assert.NotNull(result);
        Assert.Equal(expectedToken, result!.TotpToken);
    }

    [Fact]
    public async Task TotpAsync_ReturnsResponseDto()
    {
        var expectedToken = Guid.NewGuid();

        var expected = new TotpResponseDto
        {
            Token = expectedToken
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        };

        var httpClient = CreateHttpClient(response);
        var service = new AuthenticationService(httpClient);

        var request = new TotpRequestDto
        {
            TotpToken = Guid.Empty,
            Secret = "secret-key"
        };

        var result = await service.TotpAsync(request);

        Assert.NotNull(result);
        Assert.Equal(expectedToken, result!.Token);
    }
}*/