using Cryptie.Client.Features.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptie.Client.Tests.Features.Authentication.Services;

public class KeychainManagerServiceFixture
{
    public ServiceProvider ServiceProvider { get; }
    public IKeychainManagerService Service =>
        ServiceProvider.GetRequiredService<IKeychainManagerService>();

    public KeychainManagerServiceFixture()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<IKeychainManagerService, KeychainManagerService>();

        ServiceProvider = services.BuildServiceProvider();
    }
}