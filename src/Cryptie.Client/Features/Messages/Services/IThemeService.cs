using System.Collections.Generic;

namespace Cryptie.Client.Features.Messages.ViewModels;

public interface IThemeService
{
    IReadOnlyList<string> AvailableThemes { get; }
    string CurrentTheme { get; set; }
}