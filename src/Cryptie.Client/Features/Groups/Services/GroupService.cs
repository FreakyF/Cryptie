using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Groups.Services;

public class GroupService(HttpClient httpClient) : IGroupService
{
    public async Task<IReadOnlyList<Guid>> GetUserGroupsAsync(
        UserGroupsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "user/usergroups");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<UserGroupsResponseDto>(cancellationToken: cancellationToken);

        return result?.Groups ?? [];
    }

    public async Task<string?> GetGroupNameAsync(
        GetGroupNameRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "group/getName");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<GetGroupNameResponseDto>(cancellationToken: cancellationToken);

        return result?.name;
    }

    public async Task<bool> IsGroupPrivateAsync(
        IsGroupPrivateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "group/isPrivate");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
                         .ReadFromJsonAsync<IsGroupPrivateResponseDto>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("Brak danych z serwera");

        return result.IsPrivate;
    }
}