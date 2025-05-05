using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Composition.Factories;

public interface IViewModelFactory
{
    T Create<T>(IScreen hostScreen) where T : ViewModelBase;
}