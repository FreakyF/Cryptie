using Cryptie.Client.Desktop.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Factories;

public interface IViewModelFactory
{
    T Create<T>(IScreen hostScreen) where T : ViewModelBase;
}