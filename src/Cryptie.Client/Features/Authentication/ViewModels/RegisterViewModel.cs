using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using ReactiveUI;

namespace Cryptie.Client.Features.Authentication.ViewModels;

public class RegisterViewModel : RoutableViewModelBase
{
    private const string MockPrivateKey = "MOCK_PRIVATE_KEY_ABC123";
    private const string MockPublicKey = "MOCK_PUBLIC_KEY_DEF456";
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly IMapper _mapper;
    private readonly IRegistrationState _registrationState;
    private readonly IValidator<RegisterRequestDto> _validator;

    public RegisterViewModel(
        IAuthenticationService authentication,
        IShellCoordinator coordinator,
        IValidator<RegisterRequestDto> validator,
        IExceptionMessageMapper exceptionMapper,
        IMapper mapper,
        IRegistrationState registrationState)
        : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;
        _mapper = mapper;
        _registrationState = registrationState;

        var canRegister = IsValidated();

        RegisterCommand = ReactiveCommand.CreateFromTask(RegisterAsync, canRegister);

        RegisterCommand.ThrownExceptions
            .Select(exceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        GoToLoginCommand = ReactiveCommand.Create(() => _coordinator.ShowLogin());
    }

    internal RegisterModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }

    private IObservable<bool> IsValidated()
    {
        var usernameTouched = IsTouched(x => x.Model.Username);
        var displayNameTouched = IsTouched(x => x.Model.DisplayName);
        var emailTouched = IsTouched(x => x.Model.Email);
        var passwordTouched = IsTouched(x => x.Model.Password);

        var validationChanged = this.WhenAnyValue(
                x => x.Model.Username,
                x => x.Model.DisplayName,
                x => x.Model.Email,
                x => x.Model.Password
            )
            .Select(_ => ValidateDto())
            .Publish()
            .RefCount();

        validationChanged
            .CombineLatest(usernameTouched, displayNameTouched,
                emailTouched, passwordTouched,
                (result, uT, dT, eT, pT) =>
                {
                    var order = new[]
                    {
                        nameof(RegisterRequestDto.Login),
                        nameof(RegisterRequestDto.DisplayName),
                        nameof(RegisterRequestDto.Email),
                        nameof(RegisterRequestDto.Password)
                    };

                    var touched = new Dictionary<string, bool>
                    {
                        { order[0], uT },
                        { order[1], dT },
                        { order[2], eT },
                        { order[3], pT }
                    };

                    var first = result.Errors
                        .OrderBy(e => Array.IndexOf(order, e.PropertyName))
                        .FirstOrDefault(e => touched.TryGetValue(e.PropertyName, out var t) && t);

                    return first?.ErrorMessage ?? string.Empty;
                })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        return validationChanged.Select(v => v.IsValid);
    }

    private IObservable<bool> IsTouched<T>(Expression<Func<RegisterViewModel, T>> prop)
    {
        return this.WhenAnyValue(prop)
            .Skip(1)
            .Select(_ => true)
            .StartWith(false);
    }

    private ValidationResult ValidateDto()
    {
        var dto = _mapper.Map<RegisterRequestDto>(Model);
        dto.PrivateKey = MockPrivateKey;
        dto.PublicKey = MockPublicKey;
        return _validator.Validate(dto);
    }


    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<RegisterRequestDto>(Model);
        dto.PrivateKey = MockPrivateKey;
        dto.PublicKey = MockPublicKey;
        await _validator.ValidateAsync(dto, cancellationToken);

        var result = await _authentication.RegisterAsync(dto, cancellationToken);

        if (result == null)
        {
            ErrorMessage = "An error occurred. Please try again.";
            return;
        }

        if (cancellationToken.IsCancellationRequested) _coordinator.ShowRegister();

        _registrationState.LastResponse = result;
        _coordinator.ShowQrSetup();
    }
}