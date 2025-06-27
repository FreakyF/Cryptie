using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
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
        Console.WriteLine($"### GROUPS FROM API: {string.Join(", ", result?.Groups ?? [])}");

        return result?.Groups ?? [];
    }
}