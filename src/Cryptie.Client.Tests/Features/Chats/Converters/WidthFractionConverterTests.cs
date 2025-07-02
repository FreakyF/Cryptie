using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Cryptie.Client.Features.Chats.Converters;
using Xunit;

namespace Cryptie.Client.Tests.Features.Chats.Converters
{
    public class WidthFractionConverterTests
    {
        [Theory]
        [InlineData(100.0, 0.6, null, 60.0)] // default fraction
        [InlineData(200.0, 0.5, null, 100.0)] // custom fraction
        [InlineData(150.0, 0.6, "notadouble", 90.0)] // parameter invalid, use property (poprawione expected)
        [InlineData(null, 0.6, null, 0.0)] // value is null
        [InlineData("string", 0.6, null, 0.0)] // value is not double
        public void Convert_WorksAsExpected(object value, double fraction, object parameter, double expected)
        {
            var converter = new WidthFractionConverter { Fraction = fraction };
            var result = converter.Convert(value, typeof(double), parameter, CultureInfo.InvariantCulture);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertBack_ThrowsNotSupportedException()
        {
            var converter = new WidthFractionConverter();
            Assert.Throws<NotSupportedException>(() =>
                converter.ConvertBack(1.0, typeof(double), null, CultureInfo.InvariantCulture));
        }
    }
}
