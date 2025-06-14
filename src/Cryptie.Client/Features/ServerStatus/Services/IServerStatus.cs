using System.Threading;
using System.Threading.Tasks;

namespace Cryptie.Client.Features.ServerStatus.Services;

public interface IServerStatus
{
    Task GetServerStatusAsync(CancellationToken cancellationToken = default);
}