using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Account.Dependencies;
using Cryptie.Client.Features.Account.services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;

namespace Cryptie.Client.Features.Account.ViewModels;

public class AccountViewModel : RoutableViewModelBase
{
    private readonly IAccountService _account;
    private readonly IGroupSelectionState _groupSelectionState;
    private readonly IKeychainManagerService _keychain;
    private readonly ILoginState _loginState;
    private readonly IRegistrationState _registrationState;
    private readonly IShellCoordinator _shell;

    private readonly IUserState _userState;
    private readonly IValidator<UserDisplayNameRequestDto> _validator;

    private string? _username;

    public AccountViewModel(
        IScreen hostScreen,
        IKeychainManagerService keychain,
        IShellCoordinator shell,
        IAccountService accountService,
        IValidator<UserDisplayNameRequestDto> validator,
        AccountDependencies dependencies)
        : base(hostScreen)
    {
        _keychain = keychain;
        _shell = shell;
        _account = accountService;
        _validator = validator;

        _userState = dependencies.UserState;
        _groupSelectionState = dependencies.GroupSelectionState;
        _loginState = dependencies.LoginState;
        _registrationState = dependencies.RegistrationState;

        _username = _userState.Username;

        var nameTouched = this.WhenAnyValue(vm => vm.Username)
            .Skip(1)
            .Select(_ => true)
            .StartWith(false);

        var validationChanged = this.WhenAnyValue(vm => vm.Username)
            .Select(_ => ValidateDto())
            .Publish()
            .RefCount();

        var isDifferent = this.WhenAnyValue(vm => vm.Username)
            .Select(name => !string.Equals(
                name?.Trim(),
                _userState.Username?.Trim(),
                StringComparison.Ordinal
            ));

        validationChanged
            .CombineLatest(nameTouched,
                (result, touched) => touched && !result.IsValid
                    ? result.Errors[0].ErrorMessage
                    : string.Empty)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        var canChange = validationChanged
            .CombineLatest(isDifferent,
                (validation, different) => validation.IsValid && different);

        ChangeNameCommand = ReactiveCommand.CreateFromTask(
            ExecuteChangeNameAsync, canChange);

        SignOutCommand = ReactiveCommand.Create(ExecuteLogout);
    }

    public ReactiveCommand<Unit, Unit> ChangeNameCommand { get; }
    public ReactiveCommand<Unit, Unit> SignOutCommand { get; }

    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private ValidationResult ValidateDto()
    {
        var dto = new UserDisplayNameRequestDto
        {
            Name = Username ?? string.Empty
        };

        return _validator.Validate(dto);
    }

    private async Task ExecuteChangeNameAsync()
    {
        try
        {
            var tokenString = _userState.SessionToken;
            if (string.IsNullOrWhiteSpace(tokenString)
                || !Guid.TryParse(tokenString, out var token))
            {
                return;
            }

            var dto = new UserDisplayNameRequestDto
            {
                Token = token,
                Name = Username!
            };

            await _validator.ValidateAndThrowAsync(dto);
            await _account.ChangeUserDisplayNameAsync(dto);

            _userState.Username = Username;
            ErrorMessage = string.Empty;
        }
        catch
        {
            ErrorMessage = "An error occurred. Please try again.";
        }
    }

    private void ExecuteLogout()
    {
        _keychain.TryClearSessionToken(out _);

        _userState.Username = null;
        _userState.SessionToken = null;

        _groupSelectionState.SelectedGroupId = Guid.Empty;
        _groupSelectionState.SelectedGroupName = null;
        _groupSelectionState.IsGroupPrivate = false;

        _loginState.LastResponse = null;
        _registrationState.LastResponse = null;

        _shell.ResetAndShowLogin();
    }
}