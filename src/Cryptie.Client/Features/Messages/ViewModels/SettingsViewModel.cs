using System.Collections.Generic;
using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels
{
    public class SettingsViewModel : RoutableViewModelBase
    {
        private readonly IThemeService _themeService;

        private string _selectedTheme;

        public SettingsViewModel(IScreen hostScreen, IThemeService themeService)
            : base(hostScreen)
        {
            _themeService = themeService;
            _selectedTheme = _themeService.CurrentTheme;
            AvailableThemes = new List<string>(_themeService.AvailableThemes);
        }

        public List<string> AvailableThemes { get; }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value == _selectedTheme)
                    return;

                this.RaiseAndSetIfChanged(ref _selectedTheme, value);

                // przekaż zmianę do serwisu (zapisywanie + stosowanie)
                _themeService.CurrentTheme = value;
            }
        }
    }
}