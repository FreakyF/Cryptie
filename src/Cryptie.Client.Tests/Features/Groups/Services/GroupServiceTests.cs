using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Common.Features.Groups.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using Moq.Protected;
using Xunit;

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
        var httpClient = new HttpClient(handler.Object);
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
        var httpClient = new HttpClient(handler.Object);
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
        var httpClient = new HttpClient(handler.Object);
        var service = new GroupService(httpClient);
        var request = new UserGroupsRequestDto();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUserGroupsAsync(request));
    }
}

