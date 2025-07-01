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

    // public async Task<string?> GetGroupNameAsync(
    //     GetGroupNameRequestDto request,
    //     CancellationToken cancellationToken = default)
    // {
    //     using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "group/getName");
    //     httpRequest.Content = JsonContent.Create(request);
    //
    //     using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
    //     response.EnsureSuccessStatusCode();
    //
    //     var result = await response.Content
    //         .ReadFromJsonAsync<GetGroupNameResponseDto>(cancellationToken: cancellationToken);
    //
    //     return result?.name;
    // }

    public async Task<Dictionary<Guid, bool>> GetGroupsPrivacyAsync(
        IsGroupsPrivateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "group/isGroupsPrivate");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
                         .ReadFromJsonAsync<IsGroupsPrivateResponseDto>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("No data from server");

        return result.GroupStatuses;
    }

    public async Task<Dictionary<Guid, string>> GetGroupsNamesAsync(
        GetGroupsNamesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "group/groupsNames");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<GetGroupsNamesResponseDto>(cancellationToken: cancellationToken);

        return result?.GroupsNames
               ?? new Dictionary<Guid, string>();
    }
}