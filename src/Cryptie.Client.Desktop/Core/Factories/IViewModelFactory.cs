using Cryptie.Client.Desktop.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Factories;

public interface IViewModelFactory
{
    T Create<T>(IScreen hostScreen, params object[] args) where T : ViewModelBase;
}