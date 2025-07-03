using System;
using System.Globalization;
using Avalonia.Media;
using Cryptie.Client.Core.Converters;
using Xunit;

namespace Cryptie.Client.Tests.Core.Converters;

public class InitialBackgroundConverterTests
{
    private readonly InitialBackgroundConverter _converter = new();

    [Fact]
    public void Convert_NullValue_ReturnsFirstBrush()
    {
        var result = _converter.Convert(null, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result;
        Assert.Equal(Color.Parse("#F44336"), brush.Color);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("TestUser")] // różne wartości
    [InlineData("")]
    public void Convert_DifferentValues_ReturnsConsistentBrush(string input)
    {
        var result1 = _converter.Convert(input, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture);
        var result2 = _converter.Convert(input, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(result1);
        Assert.IsType<SolidColorBrush>(result2);
        Assert.Equal(((SolidColorBrush)result1).Color, ((SolidColorBrush)result2).Color);
    }

    [Fact]
    public void Convert_NonStringValue_UsesToString()
    {
        var obj = 12345;
        var result = _converter.Convert(obj, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertBack(null, typeof(object), null, CultureInfo.InvariantCulture));
    }
}

