using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public class TargetEqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        var current = value.ToString();
        var expected = parameter.ToString();

        return string.Equals(current, expected, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}