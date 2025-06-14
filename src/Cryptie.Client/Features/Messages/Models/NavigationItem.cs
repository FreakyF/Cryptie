namespace Cryptie.Client.Features.Messages.Models;

public sealed record NavigationItem(
    string FullLabel,
    string IconGlyph,
    NavigationTarget Target,
    bool IsBottom = false,
    bool IsLast = false);