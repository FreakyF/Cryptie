using System;
using Avalonia.Controls;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Locators;
using Xunit;

namespace Cryptie.Client.Tests.Core.Locators;

public class ViewLocatorTests
{
    private class DummyViewModel : ViewModelBase { }
    // DummyView przeniesiony do produkcji jako publiczny typ
    private class NotAViewModel { }

    [Fact]
    public void Build_ReturnsNull_WhenParamIsNull()
    {
        var locator = new ViewLocator();
        var result = locator.Build(null);
        Assert.Null(result);
    }
    

    [Fact]
    public void Build_ReturnsTextBlock_WhenTypeDoesNotExist()
    {
        var locator = new ViewLocator();
        var vm = new NotAViewModel();
        var result = locator.Build(vm);
        Assert.NotNull(result);
        Assert.IsType<TextBlock>(result);
        Assert.Contains("Not Found", ((TextBlock)result).Text);
    }

    [Fact]
    public void Match_ReturnsTrue_ForViewModelBase()
    {
        var locator = new ViewLocator();
        Assert.True(locator.Match(new DummyViewModel()));
    }

    [Fact]
    public void Match_ReturnsFalse_ForOtherTypes()
    {
        var locator = new ViewLocator();
        Assert.False(locator.Match(new object()));
        Assert.False(locator.Match(null));
        Assert.False(locator.Match(new NotAViewModel()));
    }
}
