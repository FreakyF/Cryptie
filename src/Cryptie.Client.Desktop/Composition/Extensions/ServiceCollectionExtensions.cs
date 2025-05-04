using System;
using Cryptie.Client.Application.Features.Authentication.Services;
using Cryptie.Client.Desktop.Composition.Factories;
using Cryptie.Client.Desktop.ViewModels;
using Cryptie.Client.Desktop.Views;
using Cryptie.Client.Infrastructure.Configuration;
using Cryptie.Client.Infrastructure.Features.Authentication.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cryptie.Client.Desktop.Composition.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientOptions>(
            configuration.GetSection("Client")
        );

        services.AddHttpClient<IAuthenticationService, AuthenticationService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUri);
            });

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}