using System.Globalization;
using Cryptie.Client.Features.Dashboard.Converters;

namespace Cryptie.Client.Tests.Features.Dashboard.Converters;

public class BottomToColumnSpanConverterTests
{
    private readonly BottomToColumnSpanConverter _converter = new();

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 2)]
    [InlineData(null, 2)]
    [InlineData("notabool", 2)]
    public void Convert_ReturnsExpectedResult(object? value, int expected)
    {
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertBack(1, typeof(bool), null, CultureInfo.InvariantCulture));
    }
}