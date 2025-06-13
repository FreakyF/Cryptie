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

    public object Convert(IList<object?> values, Type t, object? p, CultureInfo c)
    {
        if (values.Count < 2) return Zero;
        var isBottom = values[0] as bool? == true;
        var isOpenPane = values[1] as bool? == true;
        return isBottom && isOpenPane ? Open : Zero;
    }
}