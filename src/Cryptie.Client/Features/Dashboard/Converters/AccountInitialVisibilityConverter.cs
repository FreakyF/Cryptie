using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Cryptie.Client.Features.Messages.Converters;

public class AccountInitialVisibilityConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[0] is string label)
        {
            return !string.IsNullOrWhiteSpace(label) &&
                   !string.Equals(label, "Account", StringComparison.Ordinal) &&
                   !string.Equals(label, "Chats", StringComparison.Ordinal) &&
                   !string.Equals(label, "Settings", StringComparison.Ordinal);
        }

        return false;
    }
}