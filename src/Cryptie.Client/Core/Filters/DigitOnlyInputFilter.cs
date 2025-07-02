using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Cryptie.Client.Core.Filters;

public static partial class DigitOnlyInputFilter
{
    private static readonly Regex DigitRegex = MyRegex();

    public static void Attach(Control control)
    {
        if (control is not TextBox tb)
            throw new ArgumentException("DigitOnlyInputFilter can only be attached to TextBox", nameof(control));

        tb.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);

        tb.KeyDown += OnKeyDown;

        tb.GetObservable(TextBox.TextProperty)
            .Subscribe(text =>
            {
                if (string.IsNullOrEmpty(text)) return;
                var sanitized = new string(text.Where(char.IsDigit).ToArray());
                if (sanitized != text)
                    tb.Text = sanitized;
            });
    }

    private static void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (e.Text is null) return;
        if (!DigitRegex.IsMatch(e.Text))
            e.Handled = true;
    }

    private static async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key != Key.V || !e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                return;
            }

            if (sender is not TextBox tb) return;
            var clipboard = TopLevel.GetTopLevel(tb)?.Clipboard;
            if (clipboard is null) return;

            var text = await clipboard.GetTextAsync();
            if (string.IsNullOrEmpty(text)) return;

            if (text.Any(c => !char.IsDigit(c)))
                e.Handled = true;
        }
        catch (Exception)
        {
            // Swallow exception: do nothing
        }
    }

    [GeneratedRegex("^[0-9]$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MyRegex();
}