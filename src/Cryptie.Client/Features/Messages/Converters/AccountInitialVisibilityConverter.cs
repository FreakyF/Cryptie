using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Cryptie.Client.Features.Messages.Models;

namespace Cryptie.Client.Features.Messages.Converters;

public class AccountInitialVisibilityConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[0] is NavigationTarget tgt
            && values[1] is string label)
        {
            return tgt == NavigationTarget.Account
                   && !string.Equals(label, "Account", StringComparison.Ordinal);
        }

        return false;
    }
}