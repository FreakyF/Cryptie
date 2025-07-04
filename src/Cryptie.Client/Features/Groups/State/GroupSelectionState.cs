﻿using System;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.State;

public class GroupSelectionState : ReactiveObject, IGroupSelectionState
{
    private bool _isGroupPrivate;
    private Guid _selectedGroupId;
    private string? _selectedGroupName;

    public Guid SelectedGroupId
    {
        get => _selectedGroupId;
        set => this.RaiseAndSetIfChanged(ref _selectedGroupId, value);
    }

    public string? SelectedGroupName
    {
        get => _selectedGroupName;
        set => this.RaiseAndSetIfChanged(ref _selectedGroupName, value);
    }

    public bool IsGroupPrivate
    {
        get => _isGroupPrivate;
        set => this.RaiseAndSetIfChanged(ref _isGroupPrivate, value);
    }
}