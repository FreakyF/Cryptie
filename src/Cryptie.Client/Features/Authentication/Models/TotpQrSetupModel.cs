using System;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Authentication.Models;

public class TotpQrSetupModel : ReactiveObject
{
    private string _secret = string.Empty;
    private Guid _totpToken = Guid.Empty;

    public Guid TotpToken
    {
        get => _totpToken;
        set => this.RaiseAndSetIfChanged(ref _totpToken, value);
    }

    public string Secret
    {
        get => _secret;
        set => this.RaiseAndSetIfChanged(ref _secret, value);
    }
}