using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia;
using Cryptie.Client.Features.Messages.Converters;
using Xunit;

namespace Cryptie.Client.Tests.Features.Dashboard.Converters;

public class BottomPaneMarginConverterTests
{
    private readonly BottomPaneMarginConverter _converter = new();

    [Fact]
    public void Convert_ReturnsZero_WhenValuesCountLessThan3()
    {
        var values = new List<object?> { true, true };
        var result = _converter.Convert(values, typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Equal(new Thickness(0), result);
    }

    [Theory]
    [InlineData(true, true, true, 0, 0, 48, 0)]
    [InlineData(false, true, true, 0, 0, 0, 0)]
    [InlineData(true, false, true, 0, 0, 0, 0)]
    [InlineData(true, true, false, 0, 0, 0, 0)]
    [InlineData(false, false, false, 0, 0, 0, 0)]
    [InlineData(null, true, true, 0, 0, 0, 0)]
    [InlineData(true, null, true, 0, 0, 0, 0)]
    [InlineData(true, true, null, 0, 0, 0, 0)]
    public void Convert_ReturnsExpectedThickness(object? isBottom, object? isOpenPane, object? isLast, double left, double top, double right, double bottom)
    {
        var values = new List<object?> { isBottom, isOpenPane, isLast };
        var expected = new Thickness(left, top, right, bottom);
        var result = _converter.Convert(values, typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }
}

