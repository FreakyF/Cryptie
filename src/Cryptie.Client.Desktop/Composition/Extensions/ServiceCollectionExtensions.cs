using System;
using Cryptie.Client.Desktop.Composition.Factories;
using Cryptie.Client.Desktop.Composition.Locators;
using Cryptie.Client.Desktop.ViewModels;
using Cryptie.Client.Desktop.Views;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Client.Infrastructure.Configuration;
using Cryptie.Client.Infrastructure.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactiveUI;
using Splat;

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

        Locator.CurrentMutable.RegisterLazySingleton(() => new ReactiveViewLocator(), typeof(IViewLocator));

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IAppCoordinator, AppCoordinator>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<IValidator<RegisterRequestDto>, RegisterRequestValidator>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}