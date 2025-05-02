using System;
using System.Net.Http;
using Cryptie.Client.Desktop.ViewModels;
using Cryptie.Client.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptie.Client.Desktop;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7161")
            };
            return client;
        });
        
        services.AddSingleton<IAuthApiService, AuthApiService>();

        services.AddTransient<RegisterViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}