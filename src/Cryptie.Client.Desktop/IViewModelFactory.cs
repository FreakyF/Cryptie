using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop;

public interface IViewModelFactory
{
    T Create<T>(MainWindowViewModel parent) where T : ViewModelBase;
}