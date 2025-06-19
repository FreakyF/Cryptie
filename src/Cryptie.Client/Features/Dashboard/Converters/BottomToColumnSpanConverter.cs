using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public class BottomToColumnSpanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value as bool? == true ? 1 : 2;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}