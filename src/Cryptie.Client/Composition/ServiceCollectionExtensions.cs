using System;
using System.Reflection;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Locators;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Account.services;
using Cryptie.Client.Features.Account.ViewModels;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Chats.ViewModels;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Dashboard.Services;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Groups.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Client.Features.Menu.ViewModels;
using Cryptie.Client.Features.ServerStatus.Services;
using Cryptie.Client.Features.ServerStatus.ViewModels;
using Cryptie.Client.Features.Settings.Services;
using Cryptie.Client.Features.Settings.ViewModels;
using Cryptie.Client.Features.Shell.ViewModels;
using Cryptie.Client.Features.Shell.Views;
using Cryptie.Common.Features.Account.Validators;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Common.Features.UserManagement.Validators;
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

        services.AddHttpClient<IFriendsService, FriendsService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUri);
            });

        services.AddHttpClient<IAccountService, AccountService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseUri);
            });

        services.AddHttpClient<IGroupService, GroupService>()
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
        services.AddTransient<AddFriendViewModel>();
        services.AddTransient<ChatSettingsViewModel>();

        services.AddTransient<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        services.AddTransient<IValidator<LoginRequestDto>, LoginRequestValidator>();
        services.AddTransient<IValidator<TotpRequestDto>, TotpRequestValidator>();
        services.AddTransient<IValidator<AddFriendRequestDto>, AddFriendRequestValidator>();
        services.AddTransient<IValidator<UserDisplayNameRequestDto>, UserDisplayNameRequestValidator>();


        services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
        services.AddSingleton<IRegistrationState, RegistrationState>();
        services.AddSingleton<IUserState, UserState>();
        services.AddSingleton<IGroupSelectionState, GroupSelectionState>();
        services.AddSingleton<ILoginState, LoginState>();
        services.AddSingleton<IKeychainManagerService, KeychainManagerService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<MessagesService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
        services.AddSingleton<IScreen>(provider => provider.GetRequiredService<MainWindowViewModel>());
    }
}