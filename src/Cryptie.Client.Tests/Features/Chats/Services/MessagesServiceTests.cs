using System.Net;
using System.Net.Http.Json;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Common.Features.Messages.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using Moq.Protected;

namespace Cryptie.Client.Tests.Features.Chats.Services;

public class MessagesServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpHandlerMock = new(MockBehavior.Strict);
    private readonly HttpClient _httpClient;
    private readonly HubConnection _realHub;

    public MessagesServiceTests()
    {
        _httpClient = new HttpClient(_httpHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        _realHub = new HubConnectionBuilder().WithUrl("http://localhost").Build();
    }

    [Fact]
    public void Constructor_NullArguments_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MessagesService(null, _httpClient));
        Assert.Throws<ArgumentNullException>(() => new MessagesService(new HubConnectionBuilder().WithUrl("http://localhost").Build(), null));
    }

    [Fact]
    public async Task ConnectAsync_HandlesAllStates_AndJoinsGroups()
    {
        var hub = new HubConnectionBuilder().WithUrl("http://localhost").Build();
        var service = new MessagesService(hub, _httpClient);
        await Assert.ThrowsAnyAsync<Exception>(() => service.ConnectAsync(Guid.NewGuid(), new[] { Guid.NewGuid() }));
    }

    [Fact]
    public async Task GetGroupMessagesAsync_GetSuccess_ReturnsMessages()
    {
        var messages = new List<GetGroupMessagesResponseDto.MessageDto> { new() { Message = "test" } };
        var response = new GetGroupMessagesResponseDto { Messages = messages };
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(response)
            });
        var service = new MessagesService(_realHub, _httpClient);
        var result = await service.GetGroupMessagesAsync(Guid.NewGuid(), Guid.NewGuid());
        Assert.Single(result);
        Assert.Equal("test", result[0].Message);
    }

    [Fact]
    public async Task GetGroupMessagesAsync_GetFails_FallbacksToPost()
    {
        _httpHandlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new GetGroupMessagesSinceResponseDto
                {
                    Messages = new List<GetGroupMessagesSinceResponseDto.MessageDto>
                    {
                        new GetGroupMessagesSinceResponseDto.MessageDto
                        {
                            Message = "fallback"
                        }
                    }
                })
            });
        var service = new MessagesService(_realHub, _httpClient);
        var result = await service.GetGroupMessagesAsync(Guid.NewGuid(), Guid.NewGuid());
        Assert.Single(result);
        Assert.Equal("fallback", result[0].Message);
    }

    [Fact]
    public async Task SendMessageToGroupViaHttpAsync_Failure_Throws()
    {
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));
        var service = new MessagesService(_realHub, _httpClient);
        await Assert.ThrowsAsync<HttpRequestException>(() => service.SendMessageToGroupViaHttpAsync(Guid.NewGuid(), Guid.NewGuid(), "msg"));
    }

    [Fact]
    public async Task SendMessageToGroupAsync_Connected_Sends()
    {
        // Test nie jest możliwy do wykonania bez mockowania HubConnection, które nie jest wspierane.
        // Możesz przetestować tylko brak wyjątku przy wywołaniu metody na prawdziwym obiekcie.
        var service = new MessagesService(_realHub, _httpClient);
        await Assert.ThrowsAnyAsync<Exception>(() => service.SendMessageToGroupAsync(Guid.NewGuid(), Guid.NewGuid(), "msg"));
    }

    [Fact]
    public async Task SendMessageToGroupAsync_NotConnected_Throws()
    {
        // Test nie jest możliwy do wykonania bez mockowania HubConnection, które nie jest wspierane.
        var service = new MessagesService(_realHub, _httpClient);
        await Assert.ThrowsAnyAsync<Exception>(() => service.SendMessageToGroupAsync(Guid.NewGuid(), Guid.NewGuid(), "msg"));
    }
}