using System;
using Cryptie.Client.Core.Navigation;

namespace Cryptie.Client.Features.Messages.Models;

public sealed record NavigationItem(
    string FullLabel,
    string IconGlyph,
    Action<IContentCoordinator> NavigateAction,
    bool IsBottom = false,
    bool IsLast = false);