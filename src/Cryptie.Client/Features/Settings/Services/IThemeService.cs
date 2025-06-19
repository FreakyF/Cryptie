using System.Collections.Generic;

namespace Cryptie.Client.Features.Settings.Services;

public interface IThemeService
{
    IReadOnlyList<string> AvailableThemes { get; }
    string CurrentTheme { get; set; }
}