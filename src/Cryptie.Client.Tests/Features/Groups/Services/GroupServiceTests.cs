using System.Net;
using System.Net.Http.Json;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using Moq.Protected;

namespace Cryptie.Client.Tests.Features.Groups.Services;

public class GroupServiceTests
{
    [Fact]
    public async Task GetUserGroupsAsync_ReturnsGroups_WhenResponseIsValid()
    {
        // Arrange
        var expectedGroups = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var responseDto = new UserGroupsResponseDto { Groups = expectedGroups };
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseDto)
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new UserGroupsRequestDto();

        // Act
        var result = await service.GetUserGroupsAsync(request);

        // Assert
        Assert.Equal(expectedGroups, result);
    }

    [Fact]
    public async Task GetUserGroupsAsync_ReturnsEmpty_WhenGroupsIsNull()
    {
        // Arrange
        var responseDto = new UserGroupsResponseDto { Groups = null };
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseDto)
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new UserGroupsRequestDto();

        // Act
        var result = await service.GetUserGroupsAsync(request);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserGroupsAsync_Throws_WhenResponseIsNotSuccess()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new UserGroupsRequestDto();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUserGroupsAsync(request));
    }

    [Fact]
    public async Task GetGroupsPrivacyAsync_ReturnsStatuses_WhenResponseIsValid()
    {
        // Arrange
        var expectedStatuses = new Dictionary<Guid, bool>
        {
            { Guid.NewGuid(), true },
            { Guid.NewGuid(), false }
        };
        var responseDto = new IsGroupsPrivateResponseDto { GroupStatuses = expectedStatuses };
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseDto)
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new IsGroupsPrivateRequestDto();

        // Act
        var result = await service.GetGroupsPrivacyAsync(request);

        // Assert
        Assert.Equal(expectedStatuses, result);
    }

    [Fact]
    public async Task GetGroupsPrivacyAsync_Throws_WhenNoDataFromServer()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create<IsGroupsPrivateResponseDto>(null)
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new IsGroupsPrivateRequestDto();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetGroupsPrivacyAsync(request));
    }

    [Fact]
    public async Task GetGroupsNamesAsync_ReturnsNames_WhenResponseIsValid()
    {
        // Arrange
        var expectedNames = new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), "Grupa 1" },
            { Guid.NewGuid(), "Grupa 2" }
        };
        var responseDto = new GetGroupsNamesResponseDto { GroupsNames = expectedNames };
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseDto)
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new GetGroupsNamesRequestDto();

        // Act
        var result = await service.GetGroupsNamesAsync(request);

        // Assert
        Assert.Equal(expectedNames, result);
    }

    [Fact]
    public async Task GetGroupsNamesAsync_ReturnsEmpty_WhenGroupsNamesIsNull()
    {
        // Arrange
        var responseDto = new GetGroupsNamesResponseDto { GroupsNames = null };
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(responseDto)
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new GetGroupsNamesRequestDto();

        // Act
        var result = await service.GetGroupsNamesAsync(request);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGroupsNamesAsync_Throws_WhenResponseIsNotSuccess()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });
        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var service = new GroupService(httpClient);
        var request = new GetGroupsNamesRequestDto();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetGroupsNamesAsync(request));
    }
}
