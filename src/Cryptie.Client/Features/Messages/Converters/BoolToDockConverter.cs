using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public sealed class BoolToDockConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
    {
        return value as bool? ?? false ? Dock.Bottom : Dock.Top;
    }

    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
    {
        return value is Dock.Bottom;
    }
}