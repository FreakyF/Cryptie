using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public sealed class BoolToInvertConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c)
    {
        return !(value as bool? ?? false);
    }

    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c)
    {
        return !(value as bool? ?? false);
    }
}