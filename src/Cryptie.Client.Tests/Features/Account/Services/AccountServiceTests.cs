using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.Account.services;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using Moq.Protected;
using Xunit;

namespace Cryptie.Client.Tests.Features.Account.Services
{
    public class AccountServiceTests
    {
        [Fact]
        public async Task ChangeUserDisplayNameAsync_SuccessfulRequest_CallsEndpointAndEnsuresSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    Assert.Equal(HttpMethod.Post, request.Method);
                    Assert.EndsWith("user/userdisplayname", request.RequestUri.ToString());
                    var content = request.Content.ReadFromJsonAsync<UserDisplayNameRequestDto>(token).Result;
                    Assert.NotNull(content);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("http://localhost/")
            };
            var service = new AccountService(httpClient);
            var dto = new UserDisplayNameRequestDto { Name = "TestName", Token = Guid.NewGuid() };

            // Act
            await service.ChangeUserDisplayNameAsync(dto);

            // Assert
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ChangeUserDisplayNameAsync_ThrowsOnNonSuccessStatusCode()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest))
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("http://localhost/")
            };
            var service = new AccountService(httpClient);
            var dto = new UserDisplayNameRequestDto { Name = "TestName", Token = Guid.NewGuid() };

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.ChangeUserDisplayNameAsync(dto));
        }
    }
}
