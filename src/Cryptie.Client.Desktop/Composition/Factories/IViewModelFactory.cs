using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Composition.Factories;

public interface IViewModelFactory
{
    T Create<T>(MainWindowViewModel parent) where T : ViewModelBase;
    public T Create<T>() where T : ViewModelBase;
}