using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public class InitialConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var s = value as string;
        return string.IsNullOrWhiteSpace(s) ? string.Empty : s[..1].ToUpperInvariant();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}