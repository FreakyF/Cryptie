namespace Cryptie.Client.Features.Messages.Models;

public sealed record NavigationItem(
    string FullLabel,
    string IconGlyph,
    bool IsBottom = false,
    bool IsLast = false);