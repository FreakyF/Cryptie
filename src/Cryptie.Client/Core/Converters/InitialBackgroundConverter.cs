using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Cryptie.Client.Core.Converters;

public class InitialBackgroundConverter : IValueConverter
{
    private static readonly SolidColorBrush[] Brushes =
    [
        new(Color.Parse("#F44336")), // red
        new(Color.Parse("#E91E63")), // pink
        new(Color.Parse("#9C27B0")), // purple
        new(Color.Parse("#673AB7")), // deep purple
        new(Color.Parse("#3F51B5")), // indigo
        new(Color.Parse("#2196F3")), // blue
        new(Color.Parse("#03A9F4")), // light blue
        new(Color.Parse("#00BCD4")), // cyan
        new(Color.Parse("#009688")), // teal
        new(Color.Parse("#4CAF50")), // green
        new(Color.Parse("#090CAA")), // dark blue
        new(Color.Parse("#9CB400")), // shrek lime
        new(Color.Parse("#09AA56")), // bright green
        new(Color.Parse("#FFC107")), // amber
        new(Color.Parse("#FF9800")), // orange
        new(Color.Parse("#FF5722")) // deep orange
    ];

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Brushes[0];
        }

        var key = value.ToString() ?? string.Empty;
        var hash = key.Aggregate(0, (h, c) => h * 31 + c);
        var idx = Math.Abs(hash % Brushes.Length);
        return Brushes[idx];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}