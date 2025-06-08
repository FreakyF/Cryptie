using ReactiveUI;



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