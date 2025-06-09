using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Core.Factories;

public interface IViewModelFactory
{
    T Create<T>(IScreen hostScreen, params object[] args) where T : ViewModelBase;
}