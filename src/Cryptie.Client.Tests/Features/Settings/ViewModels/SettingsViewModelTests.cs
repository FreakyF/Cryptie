using Cryptie.Client.Features.Settings.Services;
using Cryptie.Client.Features.Settings.ViewModels;
using ReactiveUI;
using NSubstitute;

namespace Cryptie.Client.Tests.Features.Settings.ViewModels;

public class SettingsViewModelTests
{
    [Fact]
    public void Constructor_InitializesProperties_FromThemeService()
    {
        var themeService = Substitute.For<IThemeService>();
        themeService.AvailableThemes.Returns(["Default", "Light", "Dark"]);
        themeService.CurrentTheme.Returns("Light");
        var hostScreen = Substitute.For<IScreen>();

        var vm = new SettingsViewModel(hostScreen, themeService);

        Assert.Equal("Light", vm.SelectedTheme);
        Assert.Equal(["Default", "Light", "Dark"], vm.AvailableThemes);
    }

    [Fact]
    public void SelectedTheme_SetNewValue_RaisesPropertyChanged_And_UpdatesService()
    {
        var themeService = Substitute.For<IThemeService>();
        themeService.AvailableThemes.Returns(["A", "B"]);
        themeService.CurrentTheme.Returns("A");
        var hostScreen = Substitute.For<IScreen>();
        var vm = new SettingsViewModel(hostScreen, themeService);

        var wasRaised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(vm.SelectedTheme))
                wasRaised = true;
        };

        vm.SelectedTheme = "B";

        Assert.True(wasRaised, "Setting a new theme should raise PropertyChanged for SelectedTheme");
        themeService.Received(1).CurrentTheme = "B";
        Assert.Equal("B", vm.SelectedTheme);
    }

    [Fact]
    public void SelectedTheme_SetSameValue_DoesNotRaiseOrUpdateService()
    {
        var themeService = Substitute.For<IThemeService>();
        themeService.AvailableThemes.Returns(["X", "Y"]);
        themeService.CurrentTheme.Returns("X");
        var hostScreen = Substitute.For<IScreen>();
        var vm = new SettingsViewModel(hostScreen, themeService);

        var eventCount = 0;
        vm.PropertyChanged += (_, _) => eventCount++;

        themeService.ClearReceivedCalls();

        vm.SelectedTheme = "X";

        Assert.Equal(0, eventCount);
        themeService.DidNotReceive().CurrentTheme = Arg.Any<string>();
    }
}