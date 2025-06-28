using ReactiveUI;

namespace Cryptie.Client.Features.Groups.State;

public class GroupSelectionState : ReactiveObject, IGroupSelectionState
{
    private string? _selectedGroupName;

    public string? SelectedGroupName
    {
        get => _selectedGroupName;
        set => this.RaiseAndSetIfChanged(ref _selectedGroupName, value);
    }
}