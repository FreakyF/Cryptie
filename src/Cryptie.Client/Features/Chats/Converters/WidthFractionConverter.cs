using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Chats.Converters;

public class WidthFractionConverter : IValueConverter
{
    public double Fraction { get; set; } = 0.6;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double w)
        {
            return 0d;
        }

        var f = Fraction;

        if (parameter is string s && double.TryParse(s, out var p))
            f = p;

        return w * f;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}