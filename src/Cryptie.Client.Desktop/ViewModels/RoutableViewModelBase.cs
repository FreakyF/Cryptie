using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    public string UrlPathSegment { get; }
    public IScreen HostScreen { get; }

    protected RoutableViewModelBase(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        UrlPathSegment = GetType().Name;
    }
}