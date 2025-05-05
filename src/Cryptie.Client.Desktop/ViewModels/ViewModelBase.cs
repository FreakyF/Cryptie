using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class ViewModelBase : ReactiveObject
{
    internal string errorMessage = string.Empty;
}