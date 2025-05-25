using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Core.Base;
using Cryptie.Client.Desktop.Core.Mapping;
using Cryptie.Client.Desktop.Core.Navigation;
using Cryptie.Client.Desktop.Features.Authentication.Models;
using Cryptie.Client.Desktop.Features.Authentication.State;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using MapsterMapper;
using QRCoder;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Authentication.ViewModels;

public class TotpQrSetupViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly IMapper _mapper;
    private readonly IValidator<TotpRequestDto> _validator;

    public TotpQrSetupViewModel(
        IAuthenticationService authentication,
        IScreen hostScreen,
        IShellCoordinator coordinator,
        IValidator<TotpRequestDto> validator,
        IExceptionMessageMapper exceptionMapper,
        IMapper mapper,
        IRegistrationState registrationState)
        : base(hostScreen)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;
        _mapper = mapper;

        var registerResponse = registrationState.LastResponse!;

        Model.TotpToken = registerResponse.TotpToken;
        QrCode = GenerateQrCode(registerResponse.Secret);

        VerifyCommand = ReactiveCommand.CreateFromTask(TotpAsync);

        VerifyCommand.ThrownExceptions
            .Select(exceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);
    }

    public string QrCode { get; set; }

    internal TotpQrSetupModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> VerifyCommand { get; }

    private async Task TotpAsync(CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<TotpRequestDto>(Model);
        Console.WriteLine($"TotpToken: {dto.Secret}");
        Console.WriteLine($"TotpCode: {dto.TotpToken}");
        await _validator.ValidateAsync(dto, cancellationToken);

        var result = await _authentication.TotpAsync(dto, cancellationToken);

        if (result == null)
        {
            ErrorMessage = "An error occurred. Please try again.";
            return;
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            _coordinator.ShowQrSetup();
        }

        _coordinator.ShowRegister();
    }

    private static string GenerateQrCode(string payload)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Default);
        var qrCode = new SvgQRCode(qrCodeData);

        var modules = qrCodeData.ModuleMatrix.Count;
        var ppm = 250 / modules;

        var qrCodeAsSvg = qrCode.GetGraphic(ppm);

        return qrCodeAsSvg;
    }
}