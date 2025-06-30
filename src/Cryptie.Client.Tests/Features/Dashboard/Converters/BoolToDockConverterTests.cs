using System;
using System.Globalization;
using Avalonia.Controls;
using Cryptie.Client.Features.Messages.Converters;
using Xunit;

namespace Cryptie.Client.Tests.Features.Dashboard.Converters;

public class BoolToDockConverterTests
{
    private readonly BoolToDockConverter _converter = new();

    [Theory]
    [InlineData(true, Dock.Bottom)]
    [InlineData(false, Dock.Top)]
    [InlineData(null, Dock.Top)]
    [InlineData("notabool", Dock.Top)]
    public void Convert_ReturnsExpectedDock_ForVariousInputs(object? value, Dock expected)
    {
        var result = _converter.Convert(value, typeof(Dock), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Dock.Bottom, true)]
    [InlineData(Dock.Top, false)]
    [InlineData(null, false)]
    [InlineData("notadock", false)]
    public void ConvertBack_ReturnsExpectedBool_ForVariousInputs(object? value, bool expected)
    {
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }
}

