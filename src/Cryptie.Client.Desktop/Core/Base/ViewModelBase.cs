using ReactiveUI;



public class ViewModelBase : ReactiveObject
{
    private string _errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
}