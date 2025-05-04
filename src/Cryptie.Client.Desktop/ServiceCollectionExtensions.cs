using System;
using Cryptie.Client.Desktop.ViewModels;
using Cryptie.Client.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cryptie.Client.Desktop;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BackendOptions>(
            configuration.GetSection("Backend")
        );

        services.AddHttpClient<IAuthApiService, AuthApiService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<BackendOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUri);
            });

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}