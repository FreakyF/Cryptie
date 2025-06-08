using Cryptie.Client.Desktop.Core.Base;
using ReactiveUI;



public interface IViewModelFactory
{
    T Create<T>(IScreen hostScreen, params object[] args) where T : ViewModelBase;
}