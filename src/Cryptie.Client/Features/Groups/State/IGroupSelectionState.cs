namespace Cryptie.Client.Features.Groups.State;

public interface IGroupSelectionState
{
    string? SelectedGroupName { get; set; }
}