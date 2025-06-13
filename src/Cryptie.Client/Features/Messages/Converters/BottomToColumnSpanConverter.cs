using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public class BottomToColumnSpanConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value as bool? == true ? 1 : 2;

    public object ConvertBack(object? v, Type t, object? p, CultureInfo c)
        => throw new NotSupportedException();
}