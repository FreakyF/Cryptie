using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Account.ViewModels;

namespace Cryptie.Client.Features.Account.Views;

public partial class AccountView : ReactiveUserControl<AccountViewModel>
{
    public AccountView()
    {
        InitializeComponent();
    }
}