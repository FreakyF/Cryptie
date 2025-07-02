using System.Net;
using System.Net.Http.Json;
using Cryptie.Client.Core.Services;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using Moq.Protected;

namespace Cryptie.Client.Tests.Core
{
    public class UserDetailsServiceTests
    {
        [Fact]
        public async Task GetUsernameFromGuidAsync_ReturnsResponseDto_OnSuccess()
        {
            var expectedResponse = new NameFromGuidResponseDto { Name = "testuser" };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedResponse)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new UserDetailsService(httpClient);
            var request = new NameFromGuidRequestDto();

            var result = await service.GetUsernameFromGuidAsync(request);

            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Name, result.Name);
        }

        [Fact]
        public async Task GetUsernameFromGuidAsync_Throws_OnNonSuccessStatusCode()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new UserDetailsService(httpClient);
            var request = new NameFromGuidRequestDto();

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUsernameFromGuidAsync(request));
        }

        [Fact]
        public async Task GetUserGuidFromTokenAsync_ReturnsResponseDto_OnSuccess()
        {
            var expectedResponse = new UserGuidFromTokenResponseDto
                { Guid = Guid.Parse("b8a7e2e2-1c2d-4e3a-9b2a-1a2b3c4d5e6f") };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(expectedResponse)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new UserDetailsService(httpClient);
            var request = new UserGuidFromTokenRequestDto();

            var result = await service.GetUserGuidFromTokenAsync(request);

            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Guid, result.Guid);
        }

        [Fact]
        public async Task GetUserGuidFromTokenAsync_Throws_OnNonSuccessStatusCode()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClient.BaseAddress = new Uri("http://localhost");
            var service = new UserDetailsService(httpClient);
            var request = new UserGuidFromTokenRequestDto();

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUserGuidFromTokenAsync(request));
        }
    }
}