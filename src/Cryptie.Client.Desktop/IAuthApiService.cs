using System.Threading.Tasks;

namespace Cryptie.Client.Desktop;

public interface IAuthApiService
{
    Task RegisterAsync(RegisterRequest registerRequest);
}