using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Encryption;
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
        var pinTouched = IsTouched(x => x.Model.PinCode);
        var passwordTouched = IsTouched(x => x.Model.Password);

        var validationChanged = this.WhenAnyValue(
                x => x.Model.Username,
                x => x.Model.DisplayName,
                x => x.Model.Email,
                x => x.Model.Password,
                x => x.Model.PinCode
            )
            .Select(_ => ValidateDto())
            .Publish()
            .RefCount();

        validationChanged
            .CombineLatest(usernameTouched, displayNameTouched, emailTouched, pinTouched, passwordTouched,
                (result, uT, dT, eT, pinT, pT) =>
                {
                    var order = new[]
                    {
                        nameof(RegisterRequestDto.Login),
                        nameof(RegisterRequestDto.DisplayName),
                        nameof(RegisterRequestDto.Email),
                        nameof(RegisterRequestDto.Password),
                        nameof(RegisterModel.PinCode)
                    };

                    var touched = new Dictionary<string, bool>
                    {
                        { order[0], uT },
                        { order[1], dT },
                        { order[2], eT },
                        { order[4], pinT },
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
        => this.WhenAnyValue(prop).Skip(1).Select(_ => true).StartWith(false);

    private ValidationResult ValidateDto()
    {
        if (string.IsNullOrEmpty(Model.PinCode) || Model.PinCode.Length != 6)
        {
            return new ValidationResult([
                new ValidationFailure(nameof(RegisterModel.PinCode), "PIN must be exactly 6 digits")
            ]);
        }

        var dto = _mapper.Map<RegisterRequestDto>(Model);

        return _validator.Validate(dto);
    }

    private async Task RegisterAsync(CancellationToken cancellationToken)
    {
        var certificate = CertificateGenerator.GenerateCertificate();
        var pfxBytes = certificate.Export(X509ContentType.Pfx);
        var cerBytes = certificate.Export(X509ContentType.Cert);

        var privateBase64 = Convert.ToBase64String(pfxBytes);
        var publicBase64 = Convert.ToBase64String(cerBytes);

        var aesKeyBase64 = DeriveAesKeyFromPin(Model.PinCode);

        var encryptedPrivate = DataEncryption.EncryptDataAes(privateBase64, aesKeyBase64);

        var encryptedLogin = DataEncryption.EncryptDataAes(Model.Username, aesKeyBase64);

        var dto = _mapper.Map<RegisterRequestDto>(Model);
        dto.PrivateKey = encryptedPrivate;
        dto.PublicKey = publicBase64;
        dto.ControlValue = encryptedLogin;

        await _validator.ValidateAsync(dto, cancellationToken);
        var result = await _authentication.RegisterAsync(dto, cancellationToken);
        if (result == null)
        {
            ErrorMessage = "An error occurred. Please try again.";
            return;
        }

        if (cancellationToken.IsCancellationRequested)
            _coordinator.ShowRegister();

        _registrationState.LastResponse = result;
        _coordinator.ShowQrSetup();
    }

    private static string DeriveAesKeyFromPin(string pin)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(hash);
    }
}