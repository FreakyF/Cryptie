using System;

namespace Cryptie.Client.Features.Groups.State;

public interface IGroupSelectionState
{
    Guid SelectedGroupId { get; set; }
    string? SelectedGroupName { get; set; }
    bool IsGroupPrivate { get; set; }
}