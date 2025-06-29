using System.Reactive;
using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.AddUserToGroup.ViewModels;

public class AddUserToGroupViewModel(IScreen hostScreen) : RoutableViewModelBase(hostScreen)
{
    public ReactiveCommand<Unit, Unit> AddUserToGroupCommand { get; } = ReactiveCommand.Create(() => { });
}