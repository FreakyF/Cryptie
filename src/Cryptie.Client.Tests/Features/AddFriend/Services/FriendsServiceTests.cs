using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using Moq.Protected;
using Xunit;

namespace Cryptie.Client.Tests.Features.AddFriend.Services
{
    public class FriendsServiceTests
    {
        [Fact]
        public async Task AddFriendAsync_SendsPostRequestAndEnsuresSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            var service = new FriendsService(httpClient);
            var dto = new AddFriendRequestDto();

            // Act
            await service.AddFriendAsync(dto);

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("user/addfriend")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task AddFriendAsync_ThrowsOnNonSuccessStatusCode()
        {
            // Arrange
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

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            var service = new FriendsService(httpClient);
            var dto = new AddFriendRequestDto();

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.AddFriendAsync(dto));
        }
    }
}

