using System;
using System.Reflection;
using Cryptie.Client.Desktop.Core.Factories;
using Cryptie.Client.Desktop.Core.Locators;
using Cryptie.Client.Desktop.Core.Mapping;
using Cryptie.Client.Desktop.Core.Navigation;
using Cryptie.Client.Desktop.Features.Authentication.State;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;
using Cryptie.Client.Desktop.Features.Shell.ViewModels;
using Cryptie.Client.Desktop.Features.Shell.Views;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Client.Infrastructure.Configuration;
using Cryptie.Client.Infrastructure.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using FluentValidation;
using Mapster;
using MapsterMapper;
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

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IShellCoordinator, ShellCoordinator>();
        services.AddSingleton<IExceptionMessageMapper, ExceptionMessageMapper>();

        Locator.CurrentMutable.RegisterLazySingleton(() => new ReactiveViewLocator(), typeof(IViewLocator));

        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<TotpCodeViewModel>();
        services.AddTransient<TotpQrSetupViewModel>();

        services.AddTransient<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        services.AddTransient<IValidator<LoginRequestDto>, LoginRequestValidator>();
        services.AddTransient<IValidator<TotpRequestDto>, TotpRequestValidator>();

        services.AddSingleton<IRegistrationState, RegistrationState>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}