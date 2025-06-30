using System;
using System.Globalization;
using Cryptie.Client.Features.Messages.Converters;
using Xunit;

namespace Cryptie.Client.Tests.Features.Dashboard.Converters;

public class BoolToInvertConverterTests
{
    private readonly BoolToInvertConverter _converter = new();

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(null, true)]
    [InlineData("notabool", true)]
    public void Convert_ReturnsExpectedResult(object? value, bool expected)
    {
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(null, true)]
    [InlineData("notabool", true)]
    public void ConvertBack_ReturnsExpectedResult(object? value, bool expected)
    {
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }
}

