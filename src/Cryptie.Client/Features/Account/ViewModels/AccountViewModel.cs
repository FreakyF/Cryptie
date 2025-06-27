using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Account.services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;

namespace Cryptie.Client.Features.Account.ViewModels;

public class AccountViewModel : RoutableViewModelBase
{
    private readonly IAccountService _account;
    private readonly IKeychainManagerService _keychain;
    private readonly IShellCoordinator _shell;
    private readonly IUserState _userState;
    private readonly IValidator<UserDisplayNameRequestDto> _validator;

    private string? _username;

    public AccountViewModel(
        IScreen hostScreen,
        IKeychainManagerService keychain,
        IShellCoordinator shell,
        IUserState userState,
        IAccountService accountService,
        IValidator<UserDisplayNameRequestDto> validator)
        : base(hostScreen)
    {
        _keychain = keychain;
        _shell = shell;
        _userState = userState;
        _account = accountService;
        _validator = validator;

        _username = _userState.Username;

        var nameTouched = this.WhenAnyValue(vm => vm.Username)
            .Skip(1)
            .Select(_ => true)
            .StartWith(false);

        var validationChanged = this.WhenAnyValue(vm => vm.Username)
            .Select(_ => ValidateDto())
            .Publish()
            .RefCount();

        validationChanged
            .CombineLatest(nameTouched,
                (result, touched) => touched && !result.IsValid
                    ? result.Errors[0].ErrorMessage
                    : string.Empty)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        var canChange = validationChanged.Select(v => v.IsValid);

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
            if (!_keychain.TryGetSessionToken(out var token, out _))
                return;

            var dto = new UserDisplayNameRequestDto
            {
                Token = Guid.Parse(token),
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
        _shell.ResetAndShowLogin();
    }
}