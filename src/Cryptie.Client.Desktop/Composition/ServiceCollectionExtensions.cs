using System;
using Cryptie.Client.Desktop.Core.Factories;
using Cryptie.Client.Desktop.Core.Locators;
using Cryptie.Client.Desktop.Core.Navigation;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;
using Cryptie.Client.Desktop.Features.Shell.ViewModels;
using Cryptie.Client.Desktop.Features.Shell.Views;
using Cryptie.Client.Desktop.Mappers;
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

namespace Cryptie.Client.Desktop.Composition;

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
        services.AddSingleton<IShellCoordinator, ShellCoordinator>();
        services.AddSingleton<IExceptionMessageMapper, ExceptionMessageMapper>();

        Locator.CurrentMutable.RegisterLazySingleton(() => new ReactiveViewLocator(), typeof(IViewLocator));

        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();

        services.AddTransient<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        services.AddTransient<IValidator<LoginRequestDto>, LoginRequestValidator>();


        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}