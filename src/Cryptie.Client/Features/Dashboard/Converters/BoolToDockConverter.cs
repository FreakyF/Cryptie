using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Dashboard.Converters;

public sealed class BoolToDockConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value as bool? ?? false ? Dock.Bottom : Dock.Top;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Dock.Bottom;
    }
}