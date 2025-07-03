using System.Globalization;
using Cryptie.Client.Features.Chats.Converters;

namespace Cryptie.Client.Tests.Features.Chats.Converters
{
    public class WidthFractionConverterTests
    {
        [Theory]
        [InlineData(100.0, 0.6, null, 60.0)]
        [InlineData(200.0, 0.5, null, 100.0)]
        [InlineData(150.0, 0.6, "notadouble", 90.0)]
        [InlineData(null, 0.6, null, 0.0)]
        [InlineData("string", 0.6, null, 0.0)]
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
