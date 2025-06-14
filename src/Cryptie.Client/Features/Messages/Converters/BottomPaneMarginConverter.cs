using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public class BottomPaneMarginConverter : IMultiValueConverter
{
    private static readonly Thickness Zero = new(0);
    private static readonly Thickness Open = new(0, 0, 48, 0);

    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3) return Zero;

        var isBottom = values[0] as bool? == true;
        var isOpenPane = values[1] as bool? == true;
        var isLast = values[2] as bool? == true;

        return isBottom && isOpenPane && isLast ? Open : Zero;
    }
}