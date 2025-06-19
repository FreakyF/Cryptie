using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.Account.ViewModels;

public class AccountViewModel(IScreen hostScreen) : RoutableViewModelBase(hostScreen);