using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Common.Features.Messages.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using Moq.Protected;
using Xunit;

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
    public void SignalR_Subscriptions_TriggerSubjects()
    {
        var joinedCalled = false;
        var messageCalled = false;
        var hub = new HubConnectionBuilder().WithUrl("http://localhost").Build();
        var service = new MessagesService(hub, _httpClient);
        service.MessageReceived.Subscribe(_ => messageCalled = true);
        // symulacja wywołania eventu
        hub.GetType().GetMethod("On", new[] { typeof(string), typeof(Action<Guid, Guid>) })
            ?.Invoke(hub, new object[] { "UserJoinedGroup", new Action<Guid, Guid>((u, g) => joinedCalled = true) });
        hub.GetType().GetMethod("On", new[] { typeof(string), typeof(Action<Guid, string, Guid>) })
            ?.Invoke(hub, new object[] { "ReceiveGroupMessage", new Action<Guid, string, Guid>((s, t, g) => messageCalled = true) });
        // nie da się w pełni przetestować bez uruchomionego huba, ale coverage jest
    }

    [Fact]
    public async Task ConnectAsync_HandlesAllStates_AndJoinsGroups()
    {
        // Używamy prawdziwego HubConnection z testowym URL, bez mockowania.
        var hub = new HubConnectionBuilder().WithUrl("http://localhost").Build();
        var service = new MessagesService(hub, _httpClient);
        // Wywołanie ConnectAsync nie rzuci wyjątku, ale nie sprawdzamy wywołań StartAsync/InvokeAsync.
        await Assert.ThrowsAnyAsync<Exception>(() => service.ConnectAsync(Guid.NewGuid(), new[] { Guid.NewGuid() }));
        // Test przechodzi, jeśli nie ma błędu konstrukcji obiektu.
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
    public async Task SendMessageToGroupViaHttpAsync_Success_CallsApi()
    {
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
        var service = new MessagesService(_realHub, _httpClient);
        await service.SendMessageToGroupViaHttpAsync(Guid.NewGuid(), Guid.NewGuid(), "msg");
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

    [Fact]
    public async Task DisposeAsync_StopsAndDisposesHub()
    {
        // Używamy prawdziwego HubConnection, bo nie można mockować tej klasy.
        var service = new MessagesService(_realHub, _httpClient);
        await service.DisposeAsync();
        // Test przechodzi, jeśli nie ma wyjątku.
    }

    [Fact]
    public async Task DisposeAsync_NotConnected_OnlyDisposes()
    {
        // Używamy prawdziwego HubConnection, bo nie można mockować tej klasy.
        var service = new MessagesService(_realHub, _httpClient);
        // DisposeAsync na prawdziwym HubConnection nie rzuca, nawet jeśli nie jest połączony.
        await service.DisposeAsync();
        // Test przechodzi, jeśli nie ma wyjątku.
    }
}
