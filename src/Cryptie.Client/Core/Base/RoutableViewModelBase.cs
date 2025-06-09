using ReactiveUI;

namespace Cryptie.Client.Core.Base;

public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    protected RoutableViewModelBase(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        UrlPathSegment = GetType().Name;
    }

    public string UrlPathSegment { get; }
    public IScreen HostScreen { get; }
}