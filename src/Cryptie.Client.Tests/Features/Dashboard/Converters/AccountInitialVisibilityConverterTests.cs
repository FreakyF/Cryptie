using System.Globalization;
using Cryptie.Client.Features.Dashboard.Converters;

namespace Cryptie.Client.Tests.Features.Dashboard.Converters;

public class AccountInitialVisibilityConverterTests
{
    private readonly AccountInitialVisibilityConverter _converter = new();

    [Theory]
    [InlineData("Account", false)]
    [InlineData("Chats", false)]
    [InlineData("Settings", false)]
    [InlineData("Other", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Convert_ReturnsExpectedResult_ForVariousLabels(object? label, bool expected)
    {
        var values = new List<object?> { label };
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_ReturnsFalse_WhenFirstValueIsNotString()
    {
        var values = new List<object?> { 123 };
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.False((bool)result);
    }

    [Fact]
    public void Convert_Throws_WhenValuesListIsEmpty()
    {
        var values = new List<object?>();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture));
    }
}