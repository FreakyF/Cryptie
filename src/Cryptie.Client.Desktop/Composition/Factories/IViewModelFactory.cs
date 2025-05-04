using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Composition.Factories;

public interface IViewModelFactory
{
    public T Create<T>() where T : ViewModelBase;
}