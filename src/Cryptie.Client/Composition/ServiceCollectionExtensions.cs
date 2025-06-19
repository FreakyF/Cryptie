using System;
using System.Reflection;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Locators;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Messages.Services;
using Cryptie.Client.Features.Messages.ViewModels;
using Cryptie.Client.Features.ServerStatus.Services;
using Cryptie.Client.Features.ServerStatus.ViewModels;
using Cryptie.Client.Features.Shell.ViewModels;
using Cryptie.Client.Features.Shell.Views;
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

namespace Cryptie.Client.Composition;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientOptions>(configuration.GetSection("Client"));

        services.AddHttpClient<IServerStatus, ServerStatus>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUri);
            });

        services.AddHttpClient<IAuthenticationService, AuthenticationService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUri);
            });

        services.AddHttpClient<IUserDetailsService, UserDetailsService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUri);
            });

        var cfg = TypeAdapterConfig.GlobalSettings;
        cfg.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(cfg);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IShellCoordinator, ShellCoordinator>();
        services.AddSingleton<IContentCoordinator, ContentCoordinator>();
        services.AddSingleton<IExceptionMessageMapper, ExceptionMessageMapper>();

        Locator.CurrentMutable.RegisterLazySingleton(() => new ReactiveViewLocator(), typeof(IViewLocator));

        services.AddTransient<ServerStatusViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<TotpCodeViewModel>();
        services.AddTransient<TotpQrSetupViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<SplitViewMenuViewModel>();
        services.AddTransient<ChatsViewModel>();
        services.AddTransient<GroupsListViewModel>();
        services.AddTransient<AccountViewModel>();
        services.AddTransient<SettingsViewModel>();


        services.AddTransient<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        services.AddTransient<IValidator<LoginRequestDto>, LoginRequestValidator>();
        services.AddTransient<IValidator<TotpRequestDto>, TotpRequestValidator>();

        services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
        services.AddSingleton<IRegistrationState, RegistrationState>();
        services.AddSingleton<ILoginState, LoginState>();
        services.AddSingleton<IKeychainManagerService, KeychainManagerService>();
        services.AddSingleton<IThemeService, ThemeService>();

        services.AddSingleton<MessagesService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
        services.AddSingleton<IScreen>(provider => provider.GetRequiredService<MainWindowViewModel>());
    }
}