using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptie.Client.Features.ServerStatus.Services;

public class ServerStatus(HttpClient httpClient) : IServerStatus
{
    public async Task GetServerStatusAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "status/server");

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}