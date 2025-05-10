using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Core.Base;
using Cryptie.Client.Desktop.Core.Mapping;
using Cryptie.Client.Desktop.Core.Navigation;
using Cryptie.Client.Desktop.Features.Authentication.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Authentication.ViewModels;

public class RegisterViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly IValidator<RegisterRequestDto> _validator;
    private readonly IMapper _mapper;

    public RegisterViewModel(
        IAuthenticationService authentication,
        IShellCoordinator coordinator,
        IValidator<RegisterRequestDto> validator,
        IExceptionMessageMapper exceptionMapper,
        IMapper mapper)
        : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;
        _mapper = mapper;

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
        return _validator.Validate(dto);
    }


    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<RegisterRequestDto>(Model);

        await _validator.ValidateAsync(dto, cancellationToken);

        await _authentication.RegisterAsync(dto, cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
        {
            _coordinator.ShowLogin();
        }
    }
}